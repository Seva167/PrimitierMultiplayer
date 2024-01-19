using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimitierMultiplayerMod.Networking.Common.Models
{
    public struct PlayerState : INetSerializable
    {
        public Transform originTransform;
        public Transform headTransform;
        public Transform leftHandTransform;
        public Transform rightHandTransform;

        public void Deserialize(NetDataReader reader)
        {
            originTransform.Deserialize(reader);
            headTransform.Deserialize(reader);
            leftHandTransform.Deserialize(reader);
            rightHandTransform.Deserialize(reader);
        }

        public void Serialize(NetDataWriter writer)
        {
            originTransform.Serialize(writer);
            headTransform.Serialize(writer);
            leftHandTransform.Serialize(writer);
            rightHandTransform.Serialize(writer);
        }

        public static bool operator ==(PlayerState left, PlayerState right) => left.leftHandTransform == right.leftHandTransform && left.rightHandTransform == right.rightHandTransform && left.headTransform == right.headTransform && left.originTransform == right.originTransform;
        public static bool operator !=(PlayerState left, PlayerState right) => !(left == right);

        public override bool Equals(object obj)
        {
            if (obj is not PlayerState)
                return false;
            return this == (PlayerState)obj;
        }

        public override int GetHashCode() => HashCode.Combine(originTransform, headTransform, leftHandTransform, rightHandTransform);
    }
}
