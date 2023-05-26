using TUNING;
using UnityEngine;
using PeterHan.PLib.Buildings;
using GameStrings = STRINGS;

namespace RadiateHeatInSpace
{
    public class RadiativeTileConfig : IBuildingConfig
    {
        public const string ID = "RadiativeTile";
        public static PBuilding PBuilding = new PBuilding(ID, STRINGS.BUILDINGS.PREFABS.RADIATIVETILE.NAME)
        {
            Width = 1,
            Height = 1,
            Animation = "floor_basic_kanim",
            HP = BUILDINGS.HITPOINTS.TIER1,
            ConstructionTime = BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER0,
            Ingredients =
            {
                new BuildIngredient(MATERIALS.ANY_BUILDABLE, tier: 3),
            },
            Placement = BuildLocationRule.Tile,
            Floods = false,
            Entombs = false,
            IsSolidTile = true,
            SceneLayer = Grid.SceneLayer.TileMain,
            AlwaysOperational = true,
            AudioSize = "small",
            Category = "Base",
            SubCategory = BUILDINGS.PLANSUBCATEGORYSORTING[TileConfig.ID],
            Tech = "Smelting",
            Description = null,
            EffectText = null,
        };

        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef def = PBuilding.CreateDef();
            def.UseStructureTemperature = true;
            def.BlockTileAtlas = Assets.GetTextureAtlas("tiles_solid");
            def.BlockTilePlaceAtlas = Assets.GetTextureAtlas("tiles_solid_place");
            def.BlockTileMaterial = Assets.GetMaterial("tiles_solid");
            def.DecorBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_solid_tops_info");
            def.DecorPlaceBlockTileInfo = Assets.GetBlockTileDecorInfo("tiles_solid_tops_place_info");
            def.DragBuild = true;
            def.isKAnimTile = true;
            return def;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            base.ConfigureBuildingTemplate(go, prefab_tag);
            PBuilding.ConfigureBuildingTemplate(go);
            //GeneratedBuildings.MakeBuildingAlwaysOperational(go);
            // tile stuff
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            var simCellOccupier = go.AddOrGet<SimCellOccupier>();
            simCellOccupier.doReplaceElement = true;
            simCellOccupier.strengthMultiplier = 1.5f;
            simCellOccupier.movementSpeedMultiplier = DUPLICANTSTATS.MOVEMENT.BONUS_2;
            simCellOccupier.notifyOnMelt = true;
            go.AddOrGet<TileTemperature>();
            go.AddOrGet<KAnimGridTileVisualizer>().blockTileConnectorID = TileConfig.BlockTileConnectorID;
            go.AddOrGet<BuildingHP>().destroyOnDamaged = true;
            // where you add the state machine, i think
            //Patches.AttachHeatComponent(go, new Patches.RadiativeBuildingConfig(ID, 0.77f, 1f));
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            BuildingTemplates.DoPostConfigure(go);
            PBuilding.DoPostConfigureComplete(go);
            go.GetComponent<KPrefabID>().AddTag(GameTags.FloorTiles);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            base.DoPostConfigureUnderConstruction(go);
            go.AddOrGet<KAnimGridTileVisualizer>();
        }
    }
}