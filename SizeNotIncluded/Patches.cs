using System;
using ProcGen;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using Utils;
using ProcGenGame;

namespace SizeNotIncluded
{
    public class Patches : KMod.UserMod2
    {
        //public static float maxDensity = 2.5f;

        [HarmonyPatch(typeof(Worlds), "UpdateWorldCache")]
        public static class Worlds_UpdateWorldCache_Patch
        {
            private static void Postfix(Worlds __instance)
            {
                foreach (ProcGen.World world in __instance.worldCache.Values)
                {
                    Traverse.Create(world).Property("worldsize").SetValue(
                        new Vector2I(Options.Instance.XSize, Options.Instance.YSize));

                    foreach (var rule in world.worldTemplateRules
                        .FindAll(rule => rule.ruleId?.Equals("GenericGeysers") == true))
                    {
                        Traverse.Create(rule).Property("times").SetValue(
                            Options.Instance.GeyserCount);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(WorldGenSettings), nameof(WorldGenSettings.GetFloatSetting))]
        public static class WorldGenSettings_GetFloatSetting_Patch
        {
            private static void Postfix(string target, ref float __result)
            {
                var densityModifier = Options.Instance.DensityCapped;
                switch (target)
                {
                    case "OverworldDensityMin":
                    case "OverworldDensityMax":
                    case "OverworldAvoidRadius":
                        __result /= densityModifier;
                        break;
                }
            }
        }

        //public const float maxLiquidModifier = 2.0f;

        [HarmonyPatch(typeof(MutatedWorldData), "ApplyWorldTraits")]
        public static class MutatedWorldData_ApplyTraits_Patch
        {
            private static void Postfix(MutatedWorldData __instance)
            {
                var densityModifier = Options.Instance.DensityCapped;
                foreach (KeyValuePair<string, ElementBandConfiguration> bandConfiguration in __instance.biomes.BiomeBackgroundElementBandConfigurations)
                {
                    foreach (ElementGradient elementGradient in bandConfiguration.Value)
                    {
                        WorldTrait.ElementBandModifier modifier = new WorldTrait.ElementBandModifier();
                        Traverse.Create(modifier).Property("element").SetValue(elementGradient.content);
                        var element = ElementLoader.FindElementByName(elementGradient.content);
                        /*if (element != null)
                        {
                            if (element.id == SimHashes.Magma)
                            {
                                // no modifier for magma. breaks the bottom of the world
                                Traverse.Create(modifier).Property("massMultiplier").SetValue(1f);
                            }
                            else if (element.IsLiquid)
                            {
                                // give the player at most 2x liquids otherwise they really break out of the pockets they spawn in
                                Traverse.Create(modifier).Property("massMultiplier").SetValue(Math.Min(maxLiquidModifier, densityModifier));
                            }
                            else
                            {
                                // everything else - gasses and solid tiles should just use whatever density modifier the game has
                                Traverse.Create(modifier).Property("massMultiplier").SetValue(densityModifier);
                            }
                        }*/
                        if (element?.id != SimHashes.Magma)
                        {
                            Traverse.Create(modifier).Property("massMultiplier").SetValue(densityModifier);
                        }
                        elementGradient.Mod(modifier);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Border), nameof(Border.ConvertToMap))]
        public static class Border_ConvertToMap_Patch
        {
            private static void Prefix(Border __instance)
            {
                if (__instance.element == SettingsCache.borders["impenetrable"])
                {
                    __instance.width = Options.Instance.NeutroniumBorder(__instance);
                }
                else
                {
                    __instance.width = Options.Instance.BiomeBorder(__instance);
                }
            }
        }

        [HarmonyPatch(typeof(SettingsCache), nameof(SettingsCache.LoadWorldTraits))]
        public static class SettingsCache_LoadWorldTraits_Patch
        {
            private static void Postfix()
            {
                if (!Options.Instance.IsSmall)
                {
                    return;
                }
                var worldTraitsField = Traverse.Create(typeof(SettingsCache))
                    .Field<Dictionary<string, WorldTrait>>("worldTraits");
                var lessTraits = worldTraitsField.Value.Where(pair
                    => !pair.Value.filePath.Contains("BouldersLarge")
                    && !pair.Value.filePath.Contains("BouldersMedium")
                    && !pair.Value.filePath.Contains("BouldersMixed")
                    && !pair.Value.filePath.Contains("GlaciersLarge")).ToDictionary(pair => pair.Key, pair => pair.Value);
                // remove traits that are too big for small worlds
                worldTraitsField.Value = lessTraits;
            }
        }

        [HarmonyPatch(typeof(MobSettings), nameof(MobSettings.GetMob))]
        public static class MobSettings_GetMob_Patch
        {
            // need to keep list of defined in case something is used multiple times
            // otherwise each time it appeared we would up the density which would be ridiculous
            private static HashSet<string> patched = new HashSet<string>();

            private static void Postfix(string id, ref Mob __result)
            {
                if (__result != null)
                {
                    var name = __result.prefabName ?? __result.name;
                    if (name != null && !patched.Contains(name))
                    {
                        var densityModifier = Options.Instance.DensityCapped;
                        // surface biome gets mostly deleted. I don't know why. So just give the player lots of voles to compensate. squaring it is probably fine.
                        /*if (name.Equals("Mole") || prefabName.Equals("Mole"))
                        {
                            densityModifier *= densityModifier;
                        }*/
                        patched.Add(name);
                        Traverse.Create(__result).Property("density").SetValue(
                            new MinMax(__result.density.min * densityModifier, __result.density.max * densityModifier));
                    }
                }
            }
        }

        //this way of modifying geysers does not work
        /*[HarmonyPatch(typeof(TemplateSpawning), "SpawnTemplatesFromTemplateRules")]
        public static class TemplateSpawning_SpawnTemplatesFromTemplateRules_Patch
        {
            private static void Prefix(WorldGenSettings settings)
            {
                foreach (var rule in settings.world.worldTemplateRules
                    .FindAll(rule => rule.ruleId.Equals("GenericGeysers")))
                {
                    rule.GetType().GetProperty("times")
                        .SetValue(rule, Options.Instance.GeyserCount);
                }
            }
        }*/

        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            new POptions().RegisterOptions(this, typeof(Options));
        }

        [HarmonyPatch(typeof(Localization), nameof(Localization.Initialize))]
        private static class Localization_Initialize_Patch
        {
            private static void Postfix() => LocalizationUtils.Translate(typeof(STRINGS));
        }
    }
}
