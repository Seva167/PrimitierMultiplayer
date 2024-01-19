using LiteNetLib;
using LiteNetLib.Utils;
using PrimitierMultiplayerMod.Networking.ClientSide;
using PrimitierMultiplayerMod.Networking.Common.Models;
using PrimitierMultiplayerMod.Networking.Common.Packets;
using PrimitierMultiplayerMod.Networking.ServerSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimitierMultiplayerMod.Networking.Common
{
    public static class NetPacketController
    {
        internal static NetDataWriter nwriter;
        internal static NetPacketProcessor packetProcessor;

        internal static void Init()
        {
            nwriter = new NetDataWriter();
            packetProcessor = new NetPacketProcessor();

            packetProcessor.RegisterNestedType((writer, value) => writer.Put(value), reader => reader.GetVector3());
            packetProcessor.RegisterNestedType((writer, value) => writer.Put(value), reader => reader.GetVector2Int());
            packetProcessor.RegisterNestedType((writer, value) => writer.Put(value), reader => reader.GetQuaternion());
            packetProcessor.RegisterNestedType<Transform>();
            packetProcessor.RegisterNestedType<PlayerState>();
            packetProcessor.RegisterNestedType<SaveHeader>();
            packetProcessor.RegisterNestedType<NetCubeData>();
            packetProcessor.RegisterNestedType<NetGroupData>();

            // Server
            packetProcessor.SubscribeReusable<PMPJoinRequestC2SPacket, NetPeer>(ServerEvents.PlayerJoinRequestEvent);
            packetProcessor.SubscribeReusable<PMPPlayerUpdateStateC2SPacket, NetPeer>(ServerEvents.PlayerUpdateStateC2SEvent);
            packetProcessor.SubscribeReusable<PMPGroupUpdatePacket>(ServerEvents.GroupUpdateC2SEvent);

            packetProcessor.SubscribeReusable<PMPRequestOwnershipC2SPacket, NetPeer>(ServerEvents.RequestOwnershipC2SEvent);


            // Client
            packetProcessor.SubscribeReusable<PMPJoinAcceptS2CPacket>(ClientEvents.JoinAcceptedEvent);
            packetProcessor.SubscribeReusable<PMPPlayerUpdateStateS2CPacket>(ClientEvents.PlayerUpdateStateS2CEvent);
            packetProcessor.SubscribeReusable<PMPPlayerJoinedS2CPacket>(ClientEvents.PlayerJoinedEvent);
            packetProcessor.SubscribeReusable<PMPPlayerLeftS2CPacket>(ClientEvents.PlayerLeftEvent);

            packetProcessor.SubscribeReusable<PMPChunkResponseS2CPacket>(ClientEvents.ChunkResponseS2CEvent);
            packetProcessor.SubscribeReusable<PMPChunkDestroyS2CPacket>(ClientEvents.ChunkDestroyS2CEvent);
            packetProcessor.SubscribeReusable<PMPGroupUpdatePacket>(ClientEvents.GroupUpdateS2CEvent);

            packetProcessor.SubscribeReusable<PMPOwnershipTransferS2CPacket>(ClientEvents.OwnershipTransferS2CEvent);
        }

        public static void SendPacket<T>(NetPeer peer, T packet, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            nwriter.Reset();
            packetProcessor.Write(nwriter, packet);
            peer.Send(nwriter, deliveryMethod);
        }
    }
}
