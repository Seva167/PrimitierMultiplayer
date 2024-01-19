using HarmonyLib;
using Il2Cpp;
using PrimitierMultiplayerMod.Networking.ClientSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimitierMultiplayerMod.Patches
{
    [HarmonyPatch(typeof(TerrainGenerator), nameof(TerrainGenerator.Generate))]
    internal static class TerrainGeneratorGeneratePatch
    {
        private static void Prefix()
        {
            if (Mod.CurrentMode == MultiplayerMode.Client)
                TerrainGenerator.worldSeed = Client.seed;
        }
    }
}
