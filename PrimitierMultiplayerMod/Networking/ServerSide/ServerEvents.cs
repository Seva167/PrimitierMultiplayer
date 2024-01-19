using Il2Cpp;
using LiteNetLib;
using MelonLoader;
using PrimitierMultiplayerMod.Networking.ClientSide;
using PrimitierMultiplayerMod.Networking.Common;
using PrimitierMultiplayerMod.Networking.Common.Models;
using PrimitierMultiplayerMod.Networking.Common.Packets;
using PrimitierMultiplayerMod.RemoteInLocal;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PrimitierMultiplayerMod.Networking.ServerSide
{
    internal static class ServerEvents
    {
        public static void ConnectionRequestEvent(ConnectionRequest request)
        {
            request.AcceptIfKey(Mod.connKey);
        }

        public static void PeerConnectedEvent(NetPeer peer)
        {
            MelonLogger.Msg("Client {0}:{1} connected with PID: {2}", peer.EndPoint.Address, peer.EndPoint.Port, peer.Id);
        }

        public static void NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
        {
            NetPacketController.packetProcessor.ReadAllPackets(reader, peer);
        }

        public static void PeerDisconnectedEvent(NetPeer peer, DisconnectInfo info)
        {
            if (ServerNetPlayerManager.netPlayers.TryGetValue(peer.Id, out var leftPlr))
            {
                MelonLogger.Msg("Player {0} disconnected: {1}", leftPlr.Nickname, info.Reason);

                ServerNetPlayerManager.netPlayers.Remove(peer.Id);

                // Telling everybody that player left
                foreach (var plr in ServerNetPlayerManager.netPlayers.Values)
                {
                    plr.SendPacket(new PMPPlayerLeftS2CPacket
                    {
                        PLID = leftPlr.PLID
                    });
                }

                PMPRemotePlayerController.RemoveRemotePlayer(leftPlr.PLID);
            }
            else
            {
                MelonLogger.Msg("Peer disconnected: {0} PID: {1}", info.Reason, peer.Id);
            }
        }

        public static void PlayerJoinRequestEvent(PMPJoinRequestC2SPacket packet, NetPeer peer)
        {
            int plid = Server.GeneratePLID();

            MelonLogger.Msg("Player {0} joined, PID: {1}, PLID: {2}", packet.Nickname, peer.Id, plid);

            // Create new player in server
            var ssPlr = new ServerSidePlayer()
            {
                PLID = plid,
                Nickname = packet.Nickname,
                Peer = peer,
            };

            // Tell other players about new player
            // Tell new player about others
            foreach (var plr in ServerNetPlayerManager.netPlayers.Values)
            {
                plr.SendPacket(new PMPPlayerJoinedS2CPacket
                {
                    Nickname = packet.Nickname,
                    PLID = plid,
                });
                ssPlr.SendPacket(new PMPPlayerJoinedS2CPacket
                {
                    Nickname = plr.Nickname,
                    PLID = plr.PLID
                });
            }

            // Telling new player abour ourselves
            ssPlr.SendPacket(new PMPPlayerJoinedS2CPacket
            {
                PLID = ServerNetPlayerManager.thisPlayer.PLID,
                Nickname = ServerNetPlayerManager.thisPlayer.Nickname
            });

            // Accepting join and giving plid
            ssPlr.SendPacket(new PMPJoinAcceptS2CPacket
            {
                PLID = plid,
                SaveHeader = new SaveHeader(SaveAndLoad.ParseVersion(Application.version), TerrainGenerator.worldSeed, EnvironmentLightingManager.instance.jDayNightCycle.time)
            });

            ServerNetPlayerManager.netPlayers.Add(peer.Id, ssPlr);

            PMPRemotePlayerController.CreateNewRemotePlayer(ssPlr);
        }

        public static void PlayerUpdateStateC2SEvent(PMPPlayerUpdateStateC2SPacket packet, NetPeer peer)
        {
            if (ServerNetPlayerManager.netPlayers.TryGetValue(peer.Id, out var ssPlr))
            {
                ssPlr.State = packet.State;

                PMPRemotePlayerController.UpdateRemotePlayer(ssPlr.PLID, packet.State);

                ssPlr.ChunkPos = CubeGenerator.WorldToChunkPos(ssPlr.State.originTransform.position);
                if (ssPlr.ChunkPos != ssPlr.PrevChunkPos)
                {
                    MelonLogger.Msg("Player moved to chunk ({0}, {1})", ssPlr.ChunkPos.x, ssPlr.ChunkPos.y);

                    var gList = ServerNetChunkManager.StoreChunk(ssPlr.ChunkPos);
                    ssPlr.SendPacket(new PMPChunkResponseS2CPacket
                    {
                        ChunkPos = ssPlr.ChunkPos,
                        //GroupJson = gList.Select(e => JsonUtility.ToJson(e)).ToArray()
                        Groups = gList.ToArray(),
                        Update = false
                    }, DeliveryMethod.ReliableUnordered);

                    ssPlr.SendPacket(new PMPChunkDestroyS2CPacket
                    {
                        ChunkPos = ssPlr.PrevChunkPos
                    }, DeliveryMethod.ReliableUnordered);
                }
            }
            else
            {
                MelonLogger.Error("Received update from unregistered player!");
                peer.Disconnect();
            }
        }

        public static void RequestOwnershipC2SEvent(PMPRequestOwnershipC2SPacket packet, NetPeer peer)
        {
            if (ServerNetPlayerManager.netPlayers.TryGetValue(peer.Id, out var plr))
            {
                foreach (var grp in ChunkUtils.GetChunkGroups(plr.ChunkPos))
                {
                    var beh = grp.GetComponent<ServerGroupNetworkBehavior>();
                    if (beh.id != packet.ID)
                        continue;

                    beh.ownerPLID = plr.PLID;

                    foreach (var nPlr in ServerNetPlayerManager.netPlayers.Values)
                    {
                        nPlr.SendPacket(new PMPOwnershipTransferS2CPacket
                        {
                            ID = beh.id,
                            PLID = plr.PLID,
                            ChunkPos = plr.ChunkPos
                        });
                    }

                    break;
                }
            }
        }

        public static void GroupUpdateC2SEvent(PMPGroupUpdatePacket packet)
        {
            foreach (var grp in ChunkUtils.GetChunkGroups(packet.ChunkPos))
            {
                if (grp.GetComponent<ClientGroupNetworkBehavior>().id != packet.ID)
                    continue;

                grp.transform.position = packet.Position;
                grp.transform.rotation = packet.Rotation;
                grp.prevIsKinematic = true;
                grp.rb.isKinematic = true;
            }
        }
    }
}
