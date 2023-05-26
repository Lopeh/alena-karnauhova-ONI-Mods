using System;
using HarmonyLib;
using UnityEngine;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using PeterHan.PLib.Buildings;
using System.Reflection;
using Database;
using Utils;
using PeterHan.PLib.UI;

namespace ShinebugReactor
{
    public class Patches : KMod.UserMod2
    {
        /*[HarmonyPatch(typeof(BuildingStatusItems), "CreateStatusItems")]
        internal static class Tooltips
        {
            public static void Postfix(BuildingStatusItems __instance)
            {
                ShinebugReactor.CreatureCountStatus = Traverse.Create(__instance).Method("CreateStatusItem", new Type[]
                {
                    typeof(string), typeof(string), typeof(string), typeof(StatusItem.IconType), typeof(NotificationType), typeof(bool), typeof(HashedString), typeof(bool), typeof(int)
                }).GetValue<StatusItem>(new object[]
                {
                    "ShinebugReactorWattage", "BUILDING",
                    string.Empty, StatusItem.IconType.Info, NotificationType.Neutral,
                    false, OverlayModes.Power.ID//, true, 129022
                });
                ShinebugReactor.CreatureCountStatus.resolveStringCallback = ((str, data) =>
                {
                    ShinebugReactor shinebugReactor = (ShinebugReactor)data;
                    str = str.Replace("{Wattage}", GameUtil.GetFormattedWattage(shinebugReactor.CurrentWattage))
                    .Replace("{Rads}", GameUtil.GetFormattedRads(shinebugReactor.CurrentHEP))
                    .Replace("{creatures}", shinebugReactor.Creatures.Count.ToString());
                    return str;
                });
            }
        }*/

        [HarmonyPatch(typeof(IncubationMonitor.Instance), "UpdateIncubationState")]
        private static class IncubationMonitor_UpdateIncubationState_Patch
        {
            private static bool Prefix(IncubationMonitor.Instance __instance, bool stored)
            {
                if (stored && __instance.GetStorage()?.GetComponent<ShinebugReactor>())
                {
                    __instance.sm.isSuppressed.Set(false, __instance);
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(IncubationMonitor), "DropSelfFromStorage")]
        private static class IncubationMonitor_DropSelfFromStorage_Patch
        {
            private static bool Prefix(IncubationMonitor.Instance smi)
            {
                if (smi.GetStorage()?.GetComponent<ShinebugReactor>())
                {
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(IncubationMonitor), "SpawnBaby")]
        private static class IncubationMonitor_SpawnBaby_Patch
        {
            private static bool Prefix(IncubationMonitor.Instance smi)
            {
                ShinebugReactor reactor = smi.GetStorage()?.GetComponent<ShinebugReactor>();
                if (reactor)
                {
                    Traverse.Create(typeof(IncubationMonitor)).Method("SpawnShell", new Type[] { smi.GetType() })
                        .GetValue(new object[] { smi });
                    reactor.EggHatched(smi.gameObject);
                    //smi.GetStorage().Drop(smi.gameObject);
                    //smi.gameObject.AddTag(GameTags.StoredPrivate);
                    SaveLoader.Instance.saveManager.Unregister(smi.GetComponent<SaveLoadRoot>());
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(HighEnergyParticleDirectionSideScreen), nameof(HighEnergyParticleDirectionSideScreen.IsValidForTarget))]
        private static class HEPDirectionSideScreen_Patch
        {
            private static void Postfix(ref bool __result, GameObject target)
            {
                if (DlcManager.FeatureRadiationEnabled() && target.GetComponent<ShinebugReactor>())
                {
                    __result = true;
                }
            }
        }
        [HarmonyPatch(typeof(SingleSliderSideScreen), nameof(SingleSliderSideScreen.IsValidForTarget))]
        private static class SliderSideScreen_Patch
        {
            private static void Postfix(ref bool __result, GameObject target)
            {
                if (!DlcManager.FeatureRadiationEnabled() && target.GetComponent<ShinebugReactor>())
                {
                    __result = false;
                }
            }
        }
        [HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
        private static class DetailsScreen_Patch
        {
            private static void Postfix()
            {
                PUIUtils.AddSideScreenContent<ItemsRemovableSideScreen>();
            }
        }

        private static void FixStoragePriority(Storage storage)
        {
            if (Options.Instance.FixIncubatorPriority)
            {
                storage.onlyTransferFromLowerPriority = true;
            }
        }
        [HarmonyPatch(typeof(EggCrackerConfig), nameof(EggCrackerConfig.DoPostConfigureComplete))]
        private static class EggCrackerConfig_Patch
        {
            private static void Postfix(GameObject go)
            {
                FixStoragePriority(go.GetComponent<ComplexFabricator>().inStorage);
            }
        }
        [HarmonyPatch(typeof(EggIncubatorConfig), nameof(EggIncubatorConfig.DoPostConfigureComplete))]
        private static class EggIncubatorConfig_Patch
        {
            private static void Postfix(GameObject go)
            {
                FixStoragePriority(go.GetComponent<Storage>());
            }
        }

        [HarmonyPatch(typeof(Localization), nameof(Localization.Initialize))]
        private static class Localization_Initialize_Patch
        {
            private static void Postfix()
            {
                LocalizationUtils.Translate(typeof(STRINGS));
                LocalizationUtils.Translate(typeof(Utils.STRINGS));
            }
        }

        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            new POptions().RegisterOptions(this, typeof(Options));
            new PBuildingManager().Register(ShinebugReactorConfig.PBuilding);
        }
    }
}