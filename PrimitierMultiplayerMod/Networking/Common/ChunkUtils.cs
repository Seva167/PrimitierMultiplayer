using Il2Cpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PrimitierMultiplayerMod.Networking.Common
{
    internal static class ChunkUtils
    {
        public static RigidbodyManager[] GetChunkGroups(Vector2Int pos)
        {
            var list = new List<RigidbodyManager>();

            foreach (var rbm in RigidbodyManager.instances)
            {
                if (CubeGenerator.WorldToChunkPos(rbm.transform.position) != pos)
                    continue;

                list.Add(rbm);
            }

            return list.ToArray();
        }

        public static bool Approx(Vector3 a, Vector3 b)
        {
            return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z);
        }
    }
}
