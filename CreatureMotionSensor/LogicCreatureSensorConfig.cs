using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace Creature_Motion_Sensor
{
    public class LogicCreatureSensorConfig : IBuildingConfig
    {
        public const string ID = "LogicCreatureSensor";
        public const int RANGE = 5;

        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "critter_presence_sensor_kanim",
                BUILDINGS.HITPOINTS.TIER1, BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER2,
                BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.REFINED_METALS,
                BUILDINGS.MELTING_POINT_KELVIN.TIER1, BuildLocationRule.OnFoundationRotatable,
                BUILDINGS.DECOR.PENALTY.TIER0, NOISE_POLLUTION.NOISY.TIER0);
            buildingDef.Floodable = false;
            buildingDef.Overheatable = false;
            buildingDef.Entombable = false;
            buildingDef.AudioCategory = "Metal";
            buildingDef.ViewMode = OverlayModes.Logic.ID;
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            buildingDef.PermittedRotations = PermittedRotations.R360;
            buildingDef.AlwaysOperational = true;
            buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
            {
                LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, CellOffset.none,
                STRINGS.BUILDINGS.PREFABS.LOGICCREATURESENSOR.LOGIC_PORT,
                STRINGS.BUILDINGS.PREFABS.LOGICCREATURESENSOR.LOGIC_PORT_ACTIVE,
                STRINGS.BUILDINGS.PREFABS.LOGICCREATURESENSOR.LOGIC_PORT_INACTIVE,
                true)
            };
            GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
            return buildingDef;
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go) => AddVisualizer(go, true);

        public override void DoPostConfigureComplete(GameObject go)
        {
            LogicCreatureSensor logicCreatureSensor = go.AddOrGet<LogicCreatureSensor>();
            logicCreatureSensor.defaultState = false;
            logicCreatureSensor.manuallyControlled = false;
            //logicCreatureSensor.pickupRange = RANGE;
            AddVisualizer(go, false);
            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
        }

        private static void AddVisualizer(GameObject prefab, bool movable)
        {
            StationaryChoreRangeVisualizer choreRangeVisualizer = prefab.AddOrGet<StationaryChoreRangeVisualizer>();
            choreRangeVisualizer.x = -2;
            choreRangeVisualizer.y = 0;
            choreRangeVisualizer.width = 5;
            choreRangeVisualizer.height = 5;
            choreRangeVisualizer.movable = movable;
        }
    }
}