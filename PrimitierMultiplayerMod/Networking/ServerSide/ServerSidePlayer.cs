using LiteNetLib;
using PrimitierMultiplayerMod.Networking.Common;
using PrimitierMultiplayerMod.Networking.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PrimitierMultiplayerMod.Networking.ServerSide
{
    public class ServerSidePlayer : NetworkPlayer
    {
        public NetPeer Peer { get; set; }

        public Vector2Int ChunkPos
        {
            get => currChunkPos;
            set
            {
                PrevChunkPos = currChunkPos;
                currChunkPos = value;
            }
        }
        Vector2Int currChunkPos;
        public Vector2Int PrevChunkPos {  get; private set; }

        public void SendPacket<T>(T packet, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            NetPacketController.SendPacket(Peer, packet, deliveryMethod);
        }
    }
}
