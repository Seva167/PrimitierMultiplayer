using Il2Cpp;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PrimitierMultiplayerMod.Networking.Common
{
    public static class PacketSerializerExtensions
    {
        #region Vector3
        public static void Put(this NetDataWriter writer, Vector3 value)
        {
            writer.Put(value.x);
            writer.Put(value.y);
            writer.Put(value.z);
        }

        public static Vector3 GetVector3(this NetDataReader reader)
        {
            return new Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
        }
        #endregion

        #region Vector2
        public static void Put(this NetDataWriter writer, Vector2 value)
        {
            writer.Put(value.x);
            writer.Put(value.y);
        }

        public static Vector2 GetVector2(this NetDataReader reader)
        {
            return new Vector2(reader.GetFloat(), reader.GetFloat());
        }
        #endregion

        #region Vector2Int
        public static void Put(this NetDataWriter writer, Vector2Int value)
        {
            writer.Put(value.x);
            writer.Put(value.y);
        }

        public static Vector2Int GetVector2Int(this NetDataReader reader)
        {
            return new Vector2Int(reader.GetInt(), reader.GetInt());
        }
        #endregion

        #region Quaternion
        public static void Put(this NetDataWriter writer, Quaternion value)
        {
            writer.Put(value.x);
            writer.Put(value.y);
            writer.Put(value.z);
            writer.Put(value.w);
        }

        public static Quaternion GetQuaternion(this NetDataReader reader)
        {
            return new Quaternion(reader.GetFloat(), reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
        }
        #endregion

        #region T[]
        public static void PutArray<T>(this NetDataWriter writer, T[] array) where T : INetSerializable, new()
        {
            writer.Put(array.Length);

            foreach (var item in array)
            {
                writer.Put(item);
            }
        }

        public static T[] GetArray<T>(this NetDataReader reader) where T : struct, INetSerializable
        {
            var count = reader.GetInt();
            var array = new T[count];

            for (int i = 0; i < count; i++)
            {
                array[i] = reader.Get<T>();
            }

            return array;
        }
        #endregion
    }
}
