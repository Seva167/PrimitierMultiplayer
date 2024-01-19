using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PrimitierMultiplayerMod.Networking.Common.Models
{
    public struct Transform : INetSerializable
    {
        public Vector3 position;
        public Quaternion rotation;

        public void Deserialize(NetDataReader reader)
        {
            position = reader.GetVector3();
            rotation = reader.GetQuaternion();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(position);
            writer.Put(rotation);
        }

        public static bool operator ==(Transform left, Transform right) => left.position == right.position && left.rotation == right.rotation;
        public static bool operator !=(Transform left, Transform right) => !(left == right);

        public override bool Equals(object obj)
        {
            if (obj is not Transform)
                return false;
            return this == (Transform)obj;
        }

        public override int GetHashCode() => HashCode.Combine(position, rotation);
    }
}
