using HarmonyLib;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using PeterHan.PLib.Buildings;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Database;

namespace RadiateHeatInSpace
{
    public class Patches : KMod.UserMod2
    {
        public static void AttachHeatComponent(GameObject go, float emissivity)
        {
            var heat = go.AddOrGet<RadiateHeat>();
            heat.Emissivity = emissivity;
        }

        [HarmonyPatch(typeof(Assets), nameof(Assets.AddBuildingDef))]
        private static class Assets_AddBuildingDef
        {
            private static void Prefix(BuildingDef def)
            {
                GameObject go = def.BuildingComplete;
                if (go != null && Options.Instance.RadiativeBuildings
                        .TryGetValue(go.PrefabID().Name, out float emissivity))
                {
                    AttachHeatComponent(go, emissivity);
                }
            }
        }

        [HarmonyPatch(typeof(BuildingStatusItems), "CreateStatusItems")]
        private static class Tooltips
        {
            private static void Postfix(BuildingStatusItems __instance)
            {
                RadiateHeat.AddStatusItemsToDatabase(__instance);
            }
        }
        [HarmonyPatch(typeof(Localization), nameof(Localization.Initialize))]
        private static class Localization_Initialize_Patch
        {
            private static void Postfix() => LocalizationUtils.Translate(typeof(STRINGS));
        }
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary(false);
            new POptions().RegisterOptions(this, typeof(Options));
            PBuildingManager buildingManager = new PBuildingManager();
            buildingManager.Register(RadiativeTileConfig.PBuilding);
        }

    }
}
