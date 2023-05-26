using System;
using System.Collections.Generic;
using HarmonyLib;
using Klei.AI;
using UnityEngine;

namespace BlockDecorBehindWalls
{
    public class BlockDecorBehindWalls : KMod.UserMod2
    {
        public const string BehindWallDecorModifierID = "BehindWallDecorModifier";

        public static void UpdateBuildingsDecor(BuildingComplete building)
        {
            foreach (int cell in building.PlacementCells)
            {
                if (Grid.IsValidCell(cell))
                {
                    for (int layer = 0; layer < (int)ObjectLayer.NumLayers; ++layer)
                    {
                        GameObject go = Grid.Objects[cell, layer];
                        if (go)
                        {
                            BuildingDef def = building.Def;
                            AttributeInstance decor = go.GetComponent<DecorProvider>()?.decor;
                            if (def && (decor != null))
                            {
                                if (def.SceneLayer < Grid.SceneLayer.LogicGatesFront)
                                {
                                    //Debug.Log($"BlockDecorBehindWalls: checking {go}");
                                    int index = decor.Modifiers.FindIndex(
                                        x => x.AttributeId == BehindWallDecorModifierID);
                                    if (index != -1)
                                    {
                                        decor.Remove(decor.Modifiers[index]);
                                        //Debug.Log($"Modifier removed; decor value: {decor.GetTotalValue()}");
                                    }
                                    if (Grid.Objects[cell, (int)ObjectLayer.Backwall])
                                    {
                                        //Debug.Log($"Initial decor value: {decor.GetTotalValue()}");
                                        float modValue = decor.GetBaseValue() > 0 ? -1 : 1;
                                        AttributeModifier modifier;
                                        for (int i = 0; i < decor.Modifiers.Count; ++i)
                                        {
                                            modifier = decor.Modifiers[i];
                                            if (modifier.IsMultiplier)
                                            {
                                                modValue -= modifier.Value;
                                            }
                                        }
                                        //Debug.Log($"Modifier needed: {modValue}");
                                        decor.Add(new AttributeModifier(BehindWallDecorModifierID,
                                            modValue, STRINGS.MODIFIERS.BLOCKDECORBEHINDWALLS.DESC, true));
                                        //Debug.Log($"Modified decor value: {decor.GetTotalValue()}");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(BuildingComplete), "OnSpawn")]
        protected static class ConstructionPatches
        {
            private static void Postfix(BuildingComplete __instance)
            {
                UpdateBuildingsDecor(__instance);
            }
        }

        [HarmonyPatch(typeof(BuildingComplete), "OnCleanUp")]
        protected static class DeconstructionPatches
        {
            private static void Postfix(BuildingComplete __instance)
            {
                if (__instance.Def.ObjectLayer == ObjectLayer.Backwall)
                {
                    UpdateBuildingsDecor(__instance);
                }
            }
        }
    }
}
