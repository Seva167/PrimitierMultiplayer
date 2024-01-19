using Il2Cpp;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimitierMultiplayerMod.Networking.Common.Models
{
    public struct SaveHeader : INetSerializable
    {
        public byte[] version;
        public int seed;
        public float time;

        public SaveHeader(int[] version, int seed, float time)
        {
            this.version = version.Select(n => (byte)n).ToArray();
            this.seed = seed;
            this.time = time;
        }

        public void Deserialize(NetDataReader reader)
        {
            version = reader.GetBytesWithLength();
            seed = reader.GetInt();
            time = reader.GetFloat();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutBytesWithLength(version, 0, 3);
            writer.Put(seed);
            writer.Put(time);
        }
    }
}
