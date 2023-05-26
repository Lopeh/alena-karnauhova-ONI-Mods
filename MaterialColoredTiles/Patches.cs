using System;
using HarmonyLib;
using UnityEngine;
using Rendering;
using System.Collections.Generic;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using Utils;

namespace MaterialColoredTilesAndMore
{
    public class Patches : KMod.UserMod2
    {
        private static readonly List<string> acceptedTags = new List<string>();
        public static List<string> AcceptedTags {
            get
            {
                if (Options.Instance != null && acceptedTags.Count == 0)
                {
                    if (Options.Instance.Doors) acceptedTags.Add("Door");
                    if (Options.Instance.Walls)
                    {
                        acceptedTags.Add(ExteriorWallConfig.ID);
                        acceptedTags.Add(ThermalBlockConfig.ID);
                        acceptedTags.Add("ThermalInterfacePlate");
                    }
                    if (Options.Instance.Pipes) acceptedTags.Add("Conduit");
                    if (Options.Instance.Sculptures) acceptedTags.Add("Sculpture");
                    if (Options.Instance.Moulding) acceptedTags.Add("Moulding");
                    if (Options.Instance.LogicWires) acceptedTags.Add("LogicWire");
                    if (Options.Instance.FarmTiles)
                    {
                        acceptedTags.Add(FarmTileConfig.ID);
                        acceptedTags.Add(HydroponicFarmConfig.ID);
                    }
                }
                return acceptedTags;
            }
        }

        public static Color GetColor(PrimaryElement element)
        {
            Color color = element.Element?.substance?.colour ?? Color.clear;
            float mult = Options.Instance.Brightness;
            color *= new Color(mult, mult, mult);
            color.a = 1f;
            return color;
        }

    [HarmonyPatch(typeof(BlockTileRenderer), nameof(BlockTileRenderer.GetCellColour))]
        protected static class BlockTileRendererPatches
        {
            private static void Postfix(ref Color __result, int cell/*, SimHashes element*/)
            {
                if (!(Options.Instance.Tiles && Grid.Foundation[cell])) return;
                PrimaryElement elem = Grid.Objects[cell, (int)ObjectLayer.FoundationTile]?.GetComponent<PrimaryElement>();
                if (elem != null)
                {
                    __result *= GetColor(elem);
                }
            }
        }

        public static void ChangeBuildingColor(BuildingComplete building)
        {
            if (AcceptedTags.FindIndex(x => building.prefabid.name.Contains(x)) == -1) return;
            PrimaryElement element = building.primaryElement;
            if (element)
            {
                KAnimControllerBase kAnimController = element.GetComponent<KAnimControllerBase>();
                if (kAnimController)
                {
                    kAnimController.TintColour = GetColor(element);
                }
            }
        }

        [HarmonyPatch(typeof(BuildingComplete), "OnSpawn")]
        protected static class BuildingCompletePatches
        {
            private static void Postfix(BuildingComplete __instance)
            {
                ChangeBuildingColor(__instance);
            }
        }

        [HarmonyPatch(typeof(OverlayScreen), nameof(OverlayScreen.ToggleOverlay))]
        protected static class OverlayMenuPatches
        {
            private static void Postfix(HashedString newMode)
            {
                if (newMode.Equals(OverlayModes.None.ID))
                {
                    foreach (BuildingComplete building in Components.BuildingCompletes.Items)
                    {
                        ChangeBuildingColor(building);
                    }
                }
            }
        }

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
