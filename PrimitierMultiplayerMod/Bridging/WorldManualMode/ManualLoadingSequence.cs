using Il2Cpp;
using PrimitierMultiplayerMod.Bridging.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PrimitierMultiplayerMod.Bridging.WorldManualMode
{
    internal static class ManualLoadingSequence
    {
        public static void Load()
        {
            /*TerrainGenerator.Initialize(new Vector2Int(0, 0));

            var gr = new Il2CppSystem.Collections.Generic.List<SaveAndLoad.GroupData>();
            gr.Add(new SaveAndLoad.GroupData()
            {
                cubes = new Il2CppSystem.Collections.Generic.List<SaveAndLoad.CubeData>()
            });
            gr[0].cubes.Add(new SaveAndLoad.CubeData()
            {
                scale = new Vector3(1, 1, 1),
            });
            SaveAndLoad.chunkDict.Add(new Vector2Int(0, 0), gr);
            CubeGenerator.GenerateSavedChunk(new Vector2Int(0, 0));*/
        }

        public static SaveAndLoad.ChunkData SerializeChunk(Vector2Int position)
        {
            Il2CppSystem.Collections.Generic.List<SaveAndLoad.GroupData> data;
            if (!SaveAndLoad.chunkDict.ContainsKey(position))
            {
                var coll = new Il2CppSystem.Collections.Generic.List<Vector2Int>();
                coll.Add(position);
                SaveAndLoad.Store(coll.Cast<Il2CppSystem.Collections.Generic.ICollection<Vector2Int>>());
                data = SaveAndLoad.chunkDict[position].MemberwiseClone().Cast<Il2CppSystem.Collections.Generic.List<SaveAndLoad.GroupData>>();
                SaveAndLoad.chunkDict.Remove(position);
            }
            else
            {
                data = SaveAndLoad.chunkDict[position];
            }

            return new SaveAndLoad.ChunkData()
            {
                x = position.x,
                z = position.y,
                groups = data
            };
        }
    }
}
