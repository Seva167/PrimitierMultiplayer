using Il2Cpp;
using PrimitierMultiplayerMod.Networking.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PrimitierMultiplayerMod.Networking.Common.Packets
{
    public class PMPChunkResponseS2CPacket
    {
        public Vector2Int ChunkPos { get; set; }
        //public string[] GroupJson { get; set; }
        public NetGroupData[] Groups { get; set; }
        public bool Update { get; set; }
    }

    public class PMPChunkDestroyS2CPacket
    {
        public Vector2Int ChunkPos { get; set; }
    }

    public class PMPGroupUpdatePacket
    {
        public ulong ID { get; set; }
        public Vector2Int ChunkPos { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
    }

    public class PMPOwnershipTransferS2CPacket
    {
        public ulong ID { get; set; }
        public int PLID { get; set; }
        public Vector2Int ChunkPos { get; set; }
    }
}
