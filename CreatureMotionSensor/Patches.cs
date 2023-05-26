using HarmonyLib;
using Utils;

namespace Creature_Motion_Sensor
{
    public class Patches : KMod.UserMod2
    {
        [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
        private static class Db_Initialize_Patch
        {
            private static void Postfix()
            {
                ModUtil.AddBuildingToPlanScreen("Automation", LogicCreatureSensorConfig.ID, "sensors");
                //Debug.Log("Creature Motion Detector Loaded into Automation Building Planning Pane");
                Db.Get().Techs.Get("AnimalControl").unlockedItemIDs.Add(LogicCreatureSensorConfig.ID);
            }
        }

        [HarmonyPatch(typeof(Localization), nameof(Localization.Initialize))]
        private static class Localization_Initialize_Patch
        {
            private static void Postfix() => LocalizationUtils.Translate(typeof(STRINGS));
        }
    }
}
