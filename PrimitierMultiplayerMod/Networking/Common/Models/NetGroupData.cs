using Il2Cpp;
using LiteNetLib.Utils;
using PrimitierMultiplayerMod.Bridging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PrimitierMultiplayerMod.Networking.Common.Models
{
    public struct NetGroupData : INetSerializable
    {
        public ulong id = 0;

        public Vector3 pos;
        public Quaternion rot;

        public NetCubeData[] cubes;

        public NetGroupData(SaveAndLoad.GroupData groupData)
        {
            pos = groupData.pos;
            rot = groupData.rot;

            cubes = new NetCubeData[groupData.cubes.Count];
            for (int i = 0; i < cubes.Length; i++)
            {
                cubes[i] = new NetCubeData(groupData.cubes[i]);
            }
        }

        public SaveAndLoad.GroupData ToGroupData()
        {
            Il2CppSystem.Collections.Generic.List<SaveAndLoad.CubeData> cd = new();

            foreach (var cube in cubes)
            {
                cd.Add(cube.ToCubeData());
            }

            var gd = new SaveAndLoad.GroupData()
            {
                pos = pos,
                rot = rot,

                cubes = cd
            };
            return gd;
        }

        public void Deserialize(NetDataReader reader)
        {
            id = reader.GetULong();
            pos = reader.GetVector3();
            rot = reader.GetQuaternion();

            cubes = reader.GetArray<NetCubeData>();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(id);
            writer.Put(pos);
            writer.Put(rot);

            writer.PutArray(cubes);
        }
    }

    public struct NetCubeData : INetSerializable
    {
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;
        public float lifeRatio;
        public CubeConnector.Anchor anchor;
        public Substance substance;
        public CubeName name;
        public int[] connections;
        public float temperature;
        public bool isBurning;
        public float burnedRatio;
        public CubeAppearance.SectionState sectionState;
        public CubeAppearance.UVOffset uvOffset;
        public string[] behaviors;
        public string[] states;

        public NetCubeData(SaveAndLoad.CubeData cubeData)
        {
            pos = cubeData.pos;
            rot = cubeData.rot;
            scale = cubeData.scale;
            lifeRatio = cubeData.lifeRatio;
            anchor = cubeData.anchor;
            substance = cubeData.substance;
            name = cubeData.name;
            connections = cubeData.connections.ToArray();
            temperature = cubeData.temperature;
            isBurning = cubeData.isBurning;
            burnedRatio = cubeData.burnedRatio;
            sectionState = cubeData.sectionState;
            uvOffset = cubeData.uvOffset;
            behaviors = cubeData.behaviors.ToArray();
            states = cubeData.states.ToArray();
        }

        public SaveAndLoad.CubeData ToCubeData()
        {
            var cd = new SaveAndLoad.CubeData()
            {
                pos = pos,
                rot = rot,
                scale = scale,
                lifeRatio = lifeRatio,
                anchor = anchor,
                substance = substance,
                name = name,
                connections = connections.ToIl2CppList(),
                temperature = temperature,
                isBurning = isBurning,
                burnedRatio = burnedRatio,
                sectionState = sectionState,
                uvOffset = uvOffset,
                behaviors = behaviors.ToIl2CppList(),
                states = states.ToIl2CppList(),
            };
            return cd;
        }

        public void Deserialize(NetDataReader reader)
        {
            pos = reader.GetVector3();
            rot = reader.GetQuaternion();
            scale = reader.GetVector3();
            lifeRatio = reader.GetFloat();
            anchor = (CubeConnector.Anchor)reader.GetByte();
            substance = (Substance)reader.GetInt();
            name = (CubeName)reader.GetByte();

            connections = new int[reader.GetInt()];
            for (int i = 0; i < connections.Length; i++)
            {
                connections[i] = reader.GetInt();
            }

            temperature = reader.GetFloat();
            isBurning = reader.GetBool();
            burnedRatio = reader.GetFloat();
            sectionState = (CubeAppearance.SectionState)reader.GetByte();

            uvOffset = new CubeAppearance.UVOffset();
            uvOffset.right = reader.GetVector2();
            uvOffset.left = reader.GetVector2();
            uvOffset.top = reader.GetVector2();
            uvOffset.bottom = reader.GetVector2();
            uvOffset.front = reader.GetVector2();
            uvOffset.back = reader.GetVector2();

            behaviors = reader.GetStringArray();
            states = reader.GetStringArray();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(pos);
            writer.Put(rot);
            writer.Put(scale);
            writer.Put(lifeRatio);
            writer.Put((byte)anchor);
            writer.Put((int)substance);
            writer.Put((byte)name);

            writer.Put(connections.Length);
            foreach (var item in connections)
            {
                writer.Put(item);
            }

            writer.Put(temperature);
            writer.Put(isBurning);
            writer.Put(burnedRatio);
            writer.Put((byte)sectionState);

            writer.Put(uvOffset.right);
            writer.Put(uvOffset.left);
            writer.Put(uvOffset.top);
            writer.Put(uvOffset.bottom);
            writer.Put(uvOffset.front);
            writer.Put(uvOffset.back);

            writer.PutArray(behaviors);
            writer.PutArray(states);
        }
    }
}
