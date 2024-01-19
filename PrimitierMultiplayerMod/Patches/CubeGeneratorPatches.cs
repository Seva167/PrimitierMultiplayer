using HarmonyLib;
using Il2Cpp;
using PrimitierMultiplayerMod.Networking.ServerSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PrimitierMultiplayerMod.Patches
{
    [HarmonyPatch(typeof(CubeGenerator), nameof(CubeGenerator.FixedUpdate))]
    internal static class CubeGeneratorFixedUpdatePatch
    {
        private static bool Prefix()
        {
            return Mod.CurrentMode == MultiplayerMode.Server || Mod.CurrentMode == MultiplayerMode.None;
        }
    }

    [HarmonyPatch(typeof(CubeGenerator), nameof(CubeGenerator.DestroyChunks))]
    internal static class CubeGeneratorDestroyChunksPatch
    {
        private static void Prefix(Il2CppSystem.Collections.Generic.List<Vector2Int> destroyChunkPositions)
        {
            if (Mod.CurrentMode != MultiplayerMode.Server)
                return;

            foreach (var plr in ServerNetPlayerManager.netPlayers.Values)
            {
                if (destroyChunkPositions.Contains(plr.ChunkPos))
                    destroyChunkPositions.Remove(plr.ChunkPos);
            }
        }
    }
}
