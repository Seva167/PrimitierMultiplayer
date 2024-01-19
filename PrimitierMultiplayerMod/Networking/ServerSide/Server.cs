using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2Cpp;
using LiteNetLib;
using PrimitierMultiplayerMod.Networking.Common;
using PrimitierMultiplayerMod.Networking.Common.Packets;

namespace PrimitierMultiplayerMod.Networking.ServerSide
{
    public static class Server
    {
        static NetManager netMgr;
        static EventBasedNetListener netListener;

        public static bool IsRunning => netMgr.IsRunning;

        public static void Init()
        {
            netListener = new EventBasedNetListener();

            netListener.ConnectionRequestEvent += ServerEvents.ConnectionRequestEvent;
            netListener.PeerConnectedEvent += ServerEvents.PeerConnectedEvent;
            netListener.NetworkReceiveEvent += ServerEvents.NetworkReceiveEvent;
            netListener.PeerDisconnectedEvent += ServerEvents.PeerDisconnectedEvent;

            netMgr = new NetManager(netListener)
            {
                DisconnectTimeout = 1000000
            };
        }

        public static int GeneratePLID()
        {
            bool regen;
            int plid;

            do
            {
                regen = false;
                plid = UnityEngine.Random.Range(1, int.MaxValue);
                foreach (var key in ServerNetPlayerManager.netPlayers.Keys)
                {
                    if (key == plid)
                    {
                        regen = true;
                        break;
                    }
                }
            } while (regen);

            return plid;
        }

        public static void Start(int port)
        {
            netMgr.Start(port);
            ServerNetPlayerManager.thisPlayer = new ServerSidePlayer
            {
                Nickname = "Server",
                PLID = 0,
                Peer = null
            };
        }

        public static void Stop()
        {
            netMgr.DisconnectAll();
            netMgr.Stop();
        }

        public static void UpdatePlayerStatesS2C()
        {
            foreach (var plr in ServerNetPlayerManager.netPlayers.Values)
            {
                if (ServerNetPlayerManager.thisPlayer.State != ServerNetPlayerManager.thisPlayer.PrevState)
                {
                    plr.SendPacket(new PMPPlayerUpdateStateS2CPacket
                    {
                        PLID = ServerNetPlayerManager.thisPlayer.PLID,
                        State = ServerNetPlayerManager.thisPlayer.State
                    }, DeliveryMethod.Unreliable);
                }

                foreach (var updatePlr in ServerNetPlayerManager.netPlayers.Values.Where(p => p.PLID != plr.PLID))
                {
                    if (updatePlr.State == updatePlr.PrevState)
                        continue;

                    plr.SendPacket(new PMPPlayerUpdateStateS2CPacket
                    {
                        PLID = updatePlr.PLID,
                        State = updatePlr.State
                    }, DeliveryMethod.Unreliable);
                }
            }
        }

        public static void UpdateChunkStatesS2C()
        {
            foreach (var plr in ServerNetPlayerManager.netPlayers.Values)
            {
                var grps = ChunkUtils.GetChunkGroups(plr.ChunkPos);

                if (ServerNetChunkManager.ChunkChanged(plr.ChunkPos, grps.Length))
                {
                    var gList = ServerNetChunkManager.StoreChunk(plr.ChunkPos);
                    plr.SendPacket(new PMPChunkResponseS2CPacket
                    {
                        ChunkPos = plr.ChunkPos,
                        Groups = gList.ToArray(),
                        Update = true
                    }, DeliveryMethod.ReliableUnordered);
                }

                foreach (var grp in grps)
                {
                    if (ServerNetChunkManager.GroupChanged(grp))
                    {
                        var gList = ServerNetChunkManager.StoreChunk(plr.ChunkPos);
                        plr.SendPacket(new PMPChunkResponseS2CPacket
                        {
                            ChunkPos = plr.ChunkPos,
                            Groups = gList.ToArray(),
                            Update = true
                        }, DeliveryMethod.ReliableUnordered);
                        break;
                    }

                    var grpBeh = grp.GetComponent<ServerGroupNetworkBehavior>();
                    if (grpBeh.ownerPLID == 0)
                    {
                        plr.SendPacket(new PMPGroupUpdatePacket
                        {
                            ID = grpBeh.id,
                            ChunkPos = plr.ChunkPos,
                            Position = grp.transform.position,
                            Rotation = grp.transform.rotation
                        }, DeliveryMethod.Unreliable);
                    }
                }
            }
        }

        public static void Update()
        {
            netMgr.PollEvents();
        }

        public static void NetUpdate()
        {
            UpdatePlayerStatesS2C();
            UpdateChunkStatesS2C();
        }
    }
}
