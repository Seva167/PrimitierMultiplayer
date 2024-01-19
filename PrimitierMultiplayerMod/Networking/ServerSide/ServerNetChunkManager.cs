using Il2Cpp;
using PrimitierMultiplayerMod.Networking.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PrimitierMultiplayerMod.Networking.ServerSide
{
    internal static class ServerNetChunkManager
    {
        public static List<NetGroupData> StoreChunk(Vector2Int pos)
        {
            List<NetGroupData> gList = new();

            foreach (var rbMan in RigidbodyManager.instances)
            {
                if (CubeGenerator.WorldToChunkPos(rbMan.transform.position) != pos)
                    continue;

                gList.Add(new NetGroupData(SaveAndLoad.SerializeGroupData(rbMan, TerrainMeshGenerator.GetWorldPosOffset()))
                {
                    id = rbMan.GetComponent<ServerGroupNetworkBehavior>().id
                });
            }

            return gList;
        }

        static Dictionary<RigidbodyManager, SaveAndLoad.GroupData> groupPrevDict = new();
        public static bool GroupChanged(RigidbodyManager grp)
        {
            if (!groupPrevDict.ContainsKey(grp))
            {
                groupPrevDict.Add(grp, SaveAndLoad.SerializeGroupData(grp, TerrainMeshGenerator.GetWorldPosOffset()));
                return false;
            }

            var currGD = SaveAndLoad.SerializeGroupData(grp, TerrainMeshGenerator.GetWorldPosOffset());
            bool eqCubes = currGD.cubes.Count == groupPrevDict[grp].cubes.Count;
            for (int i = 0; i < currGD.cubes.Count; i++)
            {
                if (!CubesEqual(currGD.cubes[i], groupPrevDict[grp].cubes[i]))
                {
                    eqCubes = false;
                    break;
                }
            }

            return !eqCubes;
        }

        static bool CubesEqual(SaveAndLoad.CubeData a, SaveAndLoad.CubeData b)
        {
            bool eqPos = a.pos == b.pos;
            bool eqRot = a.rot == b.rot;
            bool eqScale = a.scale == b.scale;
            bool eqLr = a.lifeRatio == b.lifeRatio;
            bool eqAn = a.anchor == b.anchor;
            bool eqSub = a.substance == b.substance;
            bool eqName = a.name == b.name;

            bool eqConn = a.connections.Count == b.connections.Count;
            if (eqConn)
            {
                for (int i = 0; i < a.connections.Count; i++)
                {
                    if (a.connections[i] != b.connections[i])
                    {
                        eqConn = false;
                        break;
                    }
                }
            }

            bool eqTemp = a.temperature == b.temperature;
            bool eqBurn = a.isBurning == b.isBurning;
            bool eqBr = a.burnedRatio == b.burnedRatio;
            bool eqSs = a.sectionState == b.sectionState;

            bool eqBeh = a.behaviors == b.behaviors;
            if (eqBeh)
            {
                for (int i = 0; i < a.behaviors.Count; i++)
                {
                    if (!string.Equals(a.behaviors[i], b.behaviors[i], StringComparison.Ordinal))
                    {
                        eqBeh = false;
                        break;
                    }
                }
            }

            bool eqState = a.states == b.states;
            if (eqState)
            {
                for (int i = 0; i < a.states.Count; i++)
                {
                    if (!string.Equals(a.states[i], b.states[i], StringComparison.Ordinal))
                    {
                        eqState = false;
                        break;
                    }
                }
            }

            return eqPos && eqRot && eqScale && eqLr && eqAn && eqSub && eqName && eqConn && eqTemp && eqBurn && eqBr && eqSs && eqBeh && eqState;
        }

        static readonly Dictionary<Vector2Int, int> chunkPrevGCntDict = new();
        public static bool ChunkChanged(Vector2Int pos, int groupCount)
        {
            if (!chunkPrevGCntDict.ContainsKey(pos))
            {
                chunkPrevGCntDict.Add(pos, groupCount);
                return false;
            }
            if (groupCount != chunkPrevGCntDict[pos])
            {
                chunkPrevGCntDict[pos] = groupCount;
                return true;
            }

            return false;
        }
    }
}
