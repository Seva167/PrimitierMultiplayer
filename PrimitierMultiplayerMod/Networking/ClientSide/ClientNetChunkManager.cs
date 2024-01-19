using Il2Cpp;
using PrimitierMultiplayerMod.Networking.Common.Models;
using PrimitierMultiplayerMod.Networking.ServerSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PrimitierMultiplayerMod.Networking.ClientSide
{
    internal static class ClientNetChunkManager
    {
        //public static Dictionary<Vector3, ulong> idDict = new();
        public static ulong idSet = 0;

        public static void CreateChunk(NetGroupData[] grData)
        {
            //idDict.Clear();

            foreach (var grp in grData)
            {
                var pGrp = grp.ToGroupData();
                //idDict.Add(pGrp.pos, grp.id);
                idSet = grp.id;
                CubeGenerator.GenerateGroup(pGrp, TerrainMeshGenerator.GetWorldPosOffset(), Quaternion.identity, false, false);
            }

        }

        public static void DestroyChunk(Vector2Int chunk)
        {
            var destList = new Il2CppSystem.Collections.Generic.List<Vector2Int>(1);
            destList.Add(chunk);
            CubeGenerator.DestroyChunks(destList);
        }
    }
}
