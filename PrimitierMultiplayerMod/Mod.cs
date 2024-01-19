using MelonLoader;
using LiteNetLib;
using PrimitierMultiplayerMod.Networking.Common;
using PrimitierMultiplayerMod.Networking.ClientSide;
using PrimitierMultiplayerMod.Networking.ServerSide;
using PrimitierMultiplayerMod.Bridging.Player;
using UnityEngine;
using Il2CppInterop.Runtime.Injection;
using PrimitierMultiplayerMod.RemoteInLocal;
using System.Diagnostics;
using PrimitierMultiplayerMod.Bridging.WorldManualMode;
using Il2Cpp;
using PMAPI;

namespace PrimitierMultiplayerMod
{
    public class Mod : MelonMod
    {
        public const string connKey = "PRIMITIERMP";

        public static MultiplayerMode CurrentMode { get; private set; } = MultiplayerMode.None;

        Stopwatch tickrateSw = new();

        public override void OnInitializeMelon()
        {
            ClassInjector.RegisterTypeInIl2Cpp<ServerGroupNetworkBehavior>();
            ClassInjector.RegisterTypeInIl2Cpp<ClientGroupNetworkBehavior>();
            ClassInjector.RegisterTypeInIl2Cpp<PMPRemotePlayerEntity>();

            PMAPIModRegistry.InitPMAPI(this);
        }

        public void OnWorldWasLoaded()
        {
            if (CurrentMode == MultiplayerMode.Client)
            {
                var gc = CachedResources.Load<GameObject>("Prefabs/GroupedCube");
                var eg = CachedResources.Load<GameObject>("Prefabs/EmptyGroup");

                gc.AddComponent<ClientGroupNetworkBehavior>();
                eg.AddComponent<ClientGroupNetworkBehavior>();

                CubeGenerator.CancelPrevGeneration();
                EnvironmentLightingManager.instance.jDayNightCycle.time = Client.time;
            }

            if (CurrentMode == MultiplayerMode.Server)
            {
                var gc = CachedResources.Load<GameObject>("Prefabs/GroupedCube");
                var eg = CachedResources.Load<GameObject>("Prefabs/EmptyGroup");

                gc.AddComponent<ServerGroupNetworkBehavior>();
                eg.AddComponent<ServerGroupNetworkBehavior>();
            }
        }

        public override void OnApplicationQuit()
        {
            switch (CurrentMode)
            {
                case MultiplayerMode.None:
                    break;
                case MultiplayerMode.Client:
                    Client.Disconnect();
                    break;
                case MultiplayerMode.Server:
                    Server.Stop();
                    break;
            }
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            NetPacketController.Init();
            PlayerStateBridge.Init();
        }

        bool clInit = false;
        bool svInit = false;

        public override void OnFixedUpdate()
        {
            switch (CurrentMode)
            {
                case MultiplayerMode.None:
                    break;
                case MultiplayerMode.Client:
                    if (!clInit)
                    {
                        Client.Init();
                        Client.Connect("127.0.0.1", 25000);
                        tickrateSw.Start();
                        clInit = true;
                    }

                    Client.Update();
                    if (!Client.IsRunning) break;
                    UpdateClient();
                    break;
                case MultiplayerMode.Server:
                    if (!svInit)
                    {
                        Server.Init();
                        Server.Start(25000);
                        tickrateSw.Start();
                        svInit = true;
                    }

                    Server.Update();
                    if (!Server.IsRunning) break;
                    UpdateServer();
                    break;
            }

        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
                CurrentMode = MultiplayerMode.Client;
            if (Input.GetKeyDown(KeyCode.Keypad2))
                CurrentMode = MultiplayerMode.Server;

        }

        public void UpdateClient()
        {
            if (tickrateSw.ElapsedMilliseconds > 33)
            {
                tickrateSw.Restart();
                Client.NetUpdate();
                ClientNetPlayerManager.localPlayer.State = PlayerStateBridge.GetPlayerState();
            }
        }

        public void UpdateServer()
        {
            if (tickrateSw.ElapsedMilliseconds > 33)
            {
                tickrateSw.Restart();
                Server.NetUpdate();
                ServerNetPlayerManager.thisPlayer.State = PlayerStateBridge.GetPlayerState();
            }
        }
    }
}