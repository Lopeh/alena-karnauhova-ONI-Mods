using System;
using System.Collections.Generic;
using HarmonyLib;

namespace NoSublimationWhenItemsAreInStorage
{
    [HarmonyPatch(typeof(Sublimates), nameof(Sublimates.Sim200ms))]
    public class NoSublimationWhenItemsAreInStoragePatches : KMod.UserMod2
    {
        private static readonly HashSet<Tag> ExcludedBuildings = new HashSet<Tag>()
        {
            OxysconceConfig.ID,
        };

        private static bool Prefix(ref Sublimates __instance)
        {
            if (__instance.HasTag(GameTags.Stored))
            {
                Storage storage = __instance.GetComponent<Pickupable>()?.storage;
                return storage != null && ExcludedBuildings.Contains(storage.PrefabID());
            }
            return true;
        }
    }
}
