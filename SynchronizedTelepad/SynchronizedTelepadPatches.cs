using HarmonyLib;

namespace StormShark.OniMods
{
    [HarmonyPatch(typeof(Immigration), nameof(Immigration.EndImmigration))]
    public class SynchronizedTelepadPatches : KMod.UserMod2
    {
        public static readonly float day = 600f, immigrationTime = 350f;

        private static void Postfix(ref Immigration __instance)
        {
            float sinceStartOfCycle = GameClock.Instance.GetTimeSinceStartOfCycle();
            float totalWaitTime = __instance.GetTotalWaitTime();
            if (sinceStartOfCycle <= immigrationTime)
                totalWaitTime -= day;
            __instance.timeBeforeSpawn = totalWaitTime - (sinceStartOfCycle - immigrationTime);
        }
    }
}
