using TUNING;
using UnityEngine;
using PeterHan.PLib.Buildings;
using GameStrings = STRINGS;

namespace SealedContainer
{
    public class InsulatedContainerConfig : AbstractSealedContainerConfig
    {
        public const string ID = "InsulatedContainer";
        public static readonly PBuilding PBuilding
            = new PBuilding(ID, STRINGS.BUILDINGS.PREFABS.INSULATEDCONTAINER.NAME)
        {
            Width = 1,
            Height = 2,
            Animation = "storagelocker_kanim",
            HP = BUILDINGS.HITPOINTS.TIER1,
            ConstructionTime = BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER3,
            Ingredients =
                {
                    new BuildIngredient(nameof(SimHashes.SuperInsulator), tier: 4),
                    new BuildIngredient(MATERIALS.PLASTIC, tier: 2),
                },
            Placement = BuildLocationRule.OnFloor,
            Floods = false,
            DefaultPriority = 5,
            Category = "Base",
            SubCategory = BUILDINGS.PLANSUBCATEGORYSORTING[StorageLockerConfig.ID],
            AddAfter = SealedContainerConfig.ID,
            Tech = "TemperatureModulation",
            Description = null,
            EffectText = null,
        };

        public InsulatedContainerConfig()
        {
            InstancePBuilding = PBuilding;
            StorageItemModifiers = Storage.StandardInsulatedStorage;
        }

        public override BuildingDef CreateBuildingDef()
        {
            if (!Options.Instance.RequireSuperInsulator)
            {
                PBuilding.Ingredients[0] = new BuildIngredient(nameof(SimHashes.Ceramic), tier: 6);
                PBuilding.Ingredients[1] = new BuildIngredient(MATERIALS.PLASTIC, tier: 3);
            }
            return base.CreateBuildingDef();
        }
    }
}
