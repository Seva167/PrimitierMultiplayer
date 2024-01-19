using Il2Cpp;
using LiteNetLib;
using MelonLoader;
using PrimitierMultiplayerMod.Bridging.WorldManualMode;
using PrimitierMultiplayerMod.Networking.Common;
using PrimitierMultiplayerMod.Networking.Common.Packets;
using PrimitierMultiplayerMod.RemoteInLocal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PrimitierMultiplayerMod.Networking.ClientSide
{
    internal static class ClientEvents
    {
        public static void PeerConnectedEvent(NetPeer peer)
        {
            MelonLogger.Msg("Connected to server: {0}:{1}", peer.EndPoint.Address, peer.EndPoint.Port);
            
            Client.serverPeer = peer;
            NetPacketController.SendPacket(peer, new PMPJoinRequestC2SPacket
            {
                Nickname = "Client"
            });
        }

        public static void PeerDisconnectEvent(NetPeer peer, DisconnectInfo info)
        {
            MelonLogger.Msg("Disconnected from server: {0}", info.Reason);

            foreach (var plr in ClientNetPlayerManager.remotePlayers.Values)
            {
                PMPRemotePlayerController.RemoveRemotePlayer(plr.PLID);
            }
        }

        public static void JoinAcceptedEvent(PMPJoinAcceptS2CPacket packet)
        {
            MelonLogger.Msg("Joined server with PLID: {0}", packet.PLID);
            ClientNetPlayerManager.localPlayer.PLID = packet.PLID;

            int[] currVersion = SaveAndLoad.ParseVersion(Application.version);
            int[] remoteVer = packet.SaveHeader.version.Select(n => (int)n).ToArray();
            if (!currVersion.SequenceEqual(remoteVer))
            {
                MelonLogger.Error("Join error: version mismatch {0}.{1}.{2} vs {3}.{4}.{5}", currVersion[0], currVersion[1], currVersion[2], remoteVer[0], remoteVer[1], remoteVer[2]);
                Client.Disconnect();
                return;
            }

            GameObject.Find("TitleSpace").transform.Find("TitleMenu/LoadButtons/LoadButton_6").GetComponent<StartButton>().OnPress();
            Client.seed = packet.SaveHeader.seed;
            Client.time = packet.SaveHeader.time;
        }

        public static void NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
        {
            NetPacketController.packetProcessor.ReadAllPackets(reader, peer);
        }

        public static void PlayerUpdateStateS2CEvent(PMPPlayerUpdateStateS2CPacket packet)
        {
            ClientNetPlayerManager.remotePlayers[packet.PLID].State = packet.State;

            PMPRemotePlayerController.UpdateRemotePlayer(packet.PLID, packet.State);
        }

        public static void PlayerJoinedEvent(PMPPlayerJoinedS2CPacket packet)
        {
            MelonLogger.Msg("Player {0} joined with PLID: {1}", packet.Nickname, packet.PLID);
            var remPlr = new ClientSidePlayer
            {
                Nickname = packet.Nickname,
                PLID = packet.PLID
            };

            ClientNetPlayerManager.remotePlayers.Add(packet.PLID, remPlr);

            PMPRemotePlayerController.CreateNewRemotePlayer(remPlr);
        }

        public static void PlayerLeftEvent(PMPPlayerLeftS2CPacket packet)
        {
            var remPlr = ClientNetPlayerManager.remotePlayers[packet.PLID];
            ClientNetPlayerManager.remotePlayers.Remove(packet.PLID);

            PMPRemotePlayerController.RemoveRemotePlayer(packet.PLID);

            MelonLogger.Msg("Player {0} left", remPlr.Nickname);
        }

        public static void ChunkResponseS2CEvent(PMPChunkResponseS2CPacket packet)
        {
            /*foreach (var grp in packet.Groups)
            {
                CubeGenerator.GenerateGroup(grp.ToGroupData(), TerrainMeshGenerator.GetWorldPosOffset(), Quaternion.identity, false, false);
            }*/
            if (packet.Update)
                ClientNetChunkManager.DestroyChunk(packet.ChunkPos);
            ClientNetChunkManager.CreateChunk(packet.Groups);
        }

        public static void ChunkDestroyS2CEvent(PMPChunkDestroyS2CPacket packet)
        {
            ClientNetChunkManager.DestroyChunk(packet.ChunkPos);
        }

        public static void GroupUpdateS2CEvent(PMPGroupUpdatePacket packet)
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

        public static void OwnershipTransferS2CEvent(PMPOwnershipTransferS2CPacket packet)
        {
            foreach (var grp in ChunkUtils.GetChunkGroups(packet.ChunkPos))
            {
                var beh = grp.GetComponent<ClientGroupNetworkBehavior>();
                if (beh.id != packet.ID)
                    continue;

                beh.isOwnerLocal = packet.PLID == ClientNetPlayerManager.localPlayer.PLID;
                break;
            }
        }

    }
}
