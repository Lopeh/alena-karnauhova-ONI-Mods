using System;
using HarmonyLib;

namespace NoSublimationWhenItemsAreInStorage
{
    [HarmonyPatch(typeof(Sublimates), nameof(Sublimates.Sim200ms))]
    public class NoSublimationWhenItemsAreInStoragePatches : KMod.UserMod2
    {
        private static bool Prefix(ref Sublimates __instance)
        {
            if (__instance.GetComponent<Pickupable>().HasTag(GameTags.Stored))
            {
                return false;
            }
            return true;
        }
    }
}
