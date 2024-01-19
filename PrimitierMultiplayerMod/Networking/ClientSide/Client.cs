using Il2Cpp;
using LiteNetLib;
using PrimitierMultiplayerMod.Networking.Common;
using PrimitierMultiplayerMod.Networking.Common.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PrimitierMultiplayerMod.Networking.ClientSide
{
    public static class Client
    {
        static NetManager netMgr;
        static EventBasedNetListener netListener;
        internal static NetPeer serverPeer;

        internal static int seed;
        internal static float time;

        public static bool IsRunning => netMgr.IsRunning && serverPeer != null;

        public static void Init()
        {
            netListener = new EventBasedNetListener();

            netListener.PeerConnectedEvent += ClientEvents.PeerConnectedEvent;
            netListener.NetworkReceiveEvent += ClientEvents.NetworkReceiveEvent;
            netListener.PeerDisconnectedEvent += ClientEvents.PeerDisconnectEvent;

            netMgr = new NetManager(netListener)
            {
                DisconnectTimeout = 1000000
            };
        }

        public static void Connect(string ip, int port)
        {
            netMgr.Start();
            netMgr.Connect(ip, port, Mod.connKey);
        }

        public static void Disconnect()
        {
            netMgr.DisconnectAll();
            netMgr.Stop();
        }

        public static void UpdatePlayerStateC2S()
        {
            NetPacketController.SendPacket(serverPeer, new PMPPlayerUpdateStateC2SPacket
            {
                State = ClientNetPlayerManager.localPlayer.State
            }, DeliveryMethod.Unreliable);
        }

        public static void UpdateChunkStatesC2S()
        {
            var chunkPos = CubeGenerator.WorldToChunkPos(ClientNetPlayerManager.localPlayer.State.originTransform.position);
            foreach (var grp in ChunkUtils.GetChunkGroups(chunkPos))
            {
                var beh = grp.GetComponent<ClientGroupNetworkBehavior>();
                if (!beh.isOwnerLocal)
                    continue;

                NetPacketController.SendPacket(serverPeer, new PMPGroupUpdatePacket
                {
                    ID = beh.id,
                    ChunkPos = chunkPos,
                    Position = grp.transform.position,
                    Rotation = grp.transform.rotation
                }, DeliveryMethod.Unreliable);
            }
        }

        public static void Update()
        {
            netMgr.PollEvents();
        }

        public static void NetUpdate()
        {
            if (ClientNetPlayerManager.localPlayer.State != ClientNetPlayerManager.localPlayer.PrevState)
                UpdatePlayerStateC2S();

            UpdateChunkStatesC2S();
        }
    }
}
