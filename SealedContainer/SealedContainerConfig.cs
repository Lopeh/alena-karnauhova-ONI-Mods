using TUNING;
using UnityEngine;
using PeterHan.PLib.Buildings;
using GameStrings = STRINGS;

namespace SealedContainer
{
    public class SealedContainerConfig : AbstractSealedContainerConfig
    {
        public const string ID = "SealedContainer";
        public static readonly PBuilding PBuilding
            = new PBuilding(ID, STRINGS.BUILDINGS.PREFABS.SEALEDCONTAINER.NAME)
        {
            Width = 1,
            Height = 2,
            Animation = "storagelocker_kanim",
            HP = BUILDINGS.HITPOINTS.TIER1,
            ConstructionTime = BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER2,
            Ingredients =
                {
                    new BuildIngredient(MATERIALS.BUILDABLERAW, tier: 4),
                    new BuildIngredient(MATERIALS.PLASTIC, tier: 2),
                },
            Placement = BuildLocationRule.OnFloor,
            Floods = false,
            DefaultPriority = 5,
            Category = "Base",
            SubCategory = BUILDINGS.PLANSUBCATEGORYSORTING[StorageLockerConfig.ID],
            AddAfter = StorageLockerConfig.ID,
            Tech = "PressureManagement",
            Description = null,
            EffectText = null,
        };

        public SealedContainerConfig()
        {
            InstancePBuilding = PBuilding;
            StorageItemModifiers = Storage.StandardSealedStorage;
        }
    }
}
