using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace BlockDecorBehindWalls
{
    public class BlockDecorBehindWalls : KMod.UserMod2
    {
        public static bool CheckBackwall(int cell)
        {
            return Grid.IsValidCell(cell) && Grid.Objects[cell, (int)ObjectLayer.Backwall];
        }

        [HarmonyPatch(typeof(DecorProvider), "AddDecor")]
        protected static class DecorProviderPatches
        {
            private static bool Prefix(DecorProvider __instance)
            {
                BuildingComplete building = __instance.GetComponent<BuildingComplete>();
                BuildingDef def = building?.Def;
                if (def && (def.SceneLayer < Grid.SceneLayer.LogicGatesFront))
                {
                    if (building?.PlacementCells?.All(CheckBackwall) == true)
                    {
                        __instance.currDecor = 0.0f;
                        return false;
                    }
                }
                return true;
            }
        }

        public static void UpdateDecorBehindWall(BuildingComplete building)
        {
            if (building.Def?.ObjectLayer == ObjectLayer.Backwall)
            {
                foreach (int cell in building.PlacementCells.Where(Grid.IsValidCell))
                {
                    for (int layer = 0; layer < (int)ObjectLayer.NumLayers; ++layer)
                    {
                        GameObject go = Grid.Objects[cell, layer];
                        if (go)
                        {
                            BuildingDef def = go.GetComponent<BuildingComplete>()?.Def;
                            DecorProvider decorProvider = go.GetComponent<DecorProvider>();
                            if (decorProvider && def
                                && (def.SceneLayer < Grid.SceneLayer.LogicGatesFront))
                            {
                                decorProvider.Refresh();
                            }
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(BuildingComplete), "OnSpawn")]
        protected static class ConstructBackwallPatches
        {
            private static void Postfix(BuildingComplete __instance)
            {
                UpdateDecorBehindWall(__instance);
            }
        }

        [HarmonyPatch(typeof(BuildingComplete), "OnCleanUp")]
        protected static class DeconstructBackwallPatches
        {
            private static void Postfix(BuildingComplete __instance)
            {
                UpdateDecorBehindWall(__instance);
            }
        }
    }
}
