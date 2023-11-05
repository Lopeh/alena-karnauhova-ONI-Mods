using HarmonyLib;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using PeterHan.PLib.Buildings;
using Utils;

namespace SealedContainer
{
    public class Patches : KMod.UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            //new PLocalization().Register();
            new POptions().RegisterOptions(this, typeof(Options));
            PBuildingManager buildingManager = new PBuildingManager();
            buildingManager.Register(SealedContainerConfig.PBuilding);
            buildingManager.Register(InsulatedContainerConfig.PBuilding);
        }

        [HarmonyPatch(typeof(Localization), nameof(Localization.Initialize))]
        private static class Localization_Initialize_Patch
        {
            private static void Postfix() => LocalizationUtils.Translate(typeof(STRINGS));
        }
    }
}