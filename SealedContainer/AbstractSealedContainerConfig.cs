using System.Collections.Generic;
using TUNING;
using UnityEngine;
using PeterHan.PLib.Buildings;
using GameStrings = STRINGS;

namespace SealedContainer
{
    public abstract class AbstractSealedContainerConfig : IBuildingConfig
    {
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            //SoundEventVolumeCache.instance.AddVolume("storagelocker_kanim", "StorageLocker_Hit_metallic_low", NOISE_POLLUTION.NOISY.TIER1);
            //Prioritizable.AddRef(go);
            Storage storage = go.AddOrGet<Storage>();

            //Changes here
            storage.capacityKg = Options.Instance.Capacity;

            storage.showInUI = true;
            storage.allowItemRemoval = true;
            storage.showDescriptor = true;
            storage.storageFilters = STORAGEFILTERS.NOT_EDIBLE_SOLIDS;
            storage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
            storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
            storage.showCapacityStatusItem = true;
            storage.showCapacityAsMainStatus = true;
            go.AddOrGet<CopyBuildingSettings>().copyGroupTag = GameTags.StorageLocker;
            go.AddOrGet<SealedContainer>();
            go.AddOrGet<UserNameable>();
            go.AddOrGetDef<RocketUsageRestriction.Def>();
            go.AddOrGetDef<StorageController.Def>();
        }
    }
}
