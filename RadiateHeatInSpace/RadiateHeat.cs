using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PeterHan.PLib.Options;
using static STRINGS.UI;

namespace RadiateHeatInSpace
{
    //[SerializationConfig(MemberSerialization.OptIn)]
    [SkipSaveFileSerialization]
    public class RadiateHeat : KMonoBehaviour
    {
        public const float StefanBoltzmannConstant = 5.67e-8f;
        public const float MinimumTemperature = 3f;
        public static StatusItem NotInSpaceStatus = new StatusItem("RADIATEHEAT_NOTINSPACE", "BUILDING",
            string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.HeatFlow.ID);
        public static StatusItem TooColdStatus = new StatusItem("RADIATEHEAT_TOOCOLD", "BUILDING",
            string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.HeatFlow.ID);
        public static StatusItem RadiatingStatus = new StatusItem("RADIATEHEAT_RADIATING", "BUILDING",
            string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.HeatFlow.ID,
            resolve_string_callback: ((str, data) =>
            {
                float value = ((RadiateHeat)data).CurrentCooling;
                str = string.Format(str, GameUtil.GetFormattedHeatEnergyRate(value));
                return str;
            }));
        public static StatusItem NotInSunlightStatus = new StatusItem("RADIATEHEAT_NOTINSUNLIGHT", "BUILDING",
            string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.HeatFlow.ID);
        public static StatusItem TooHotStatus = new StatusItem("RADIATEHEAT_TOOHOT", "BUILDING",
            string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.HeatFlow.ID);
        public static StatusItem AbsorbingStatus = new StatusItem("RADIATEHEAT_ABSORBING", "BUILDING",
            string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.HeatFlow.ID,
            resolve_string_callback: ((str, data) =>
            {
                float value = ((RadiateHeat)data).CurrentHeating;
                str = string.Format(str, GameUtil.GetFormattedHeatEnergyRate(value));
                return str;
            }));
        public float Emissivity = 0.9f;
        public float SurfaceArea = 1f;
        [MyCmpReq]
        protected BuildingComplete building;
        [MyCmpReq]
        protected KSelectable selectable; // does tooltip-related stuff
        [MyCmpReq]
        protected PrimaryElement element;
        protected HandleVector<int>.Handle structureTemperature;
        protected Radiate.Instance RadiateSMI;
        protected Absorb.Instance AbsorbSMI;

        public float CurrentCooling, CurrentHeating;

        public bool CheckInSpace()
        {
            // Check whether in space
            foreach (int cell in building.PlacementCells)
            {
                if (Game.Instance.world.zoneRenderData.GetSubWorldZoneType(cell) != ProcGen.SubWorld.ZoneType.Space)
                    return false;
            }
            return true;
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Db.Get().BuildingStatusItems.Add(NotInSpaceStatus);
            Db.Get().BuildingStatusItems.Add(TooColdStatus);
            Db.Get().BuildingStatusItems.Add(RadiatingStatus);
            Db.Get().BuildingStatusItems.Add(NotInSunlightStatus);
            Db.Get().BuildingStatusItems.Add(TooHotStatus);
            Db.Get().BuildingStatusItems.Add(AbsorbingStatus);
            BuildingDef def = building.Def;
            //if (!def) return;
            SurfaceArea = def.HeightInCells * def.WidthInCells;
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();

            structureTemperature = GameComps.StructureTemperatures.GetHandle(gameObject);
            if (!structureTemperature.IsValid()) return;
            if (CheckInSpace())
            {
                RadiateSMI = new Radiate.Instance(this);
                RadiateSMI.StartSM();
                if (Options.Instance.SunlightHeat)
                {
                    AbsorbSMI = new Absorb.Instance(this);
                    AbsorbSMI.StartSM();
                }
            }
        }

        protected override void OnCleanUp()
        {
            AbsorbSMI?.StopSM("cleanup");
            RadiateSMI?.StopSM("cleanup");
            base.OnCleanUp();
        }

        public static class Radiate
        {
            public class States : GameStateMachine<States, Instance, RadiateHeat>
            {
                public class OperationalStates : State
                {
                    public State radiating;
                    public State tooCold;
                }
                public State notInSpace;
                public OperationalStates operational;
                public override void InitializeStates(out BaseState default_state)
                {
                    default_state = notInSpace;
                    notInSpace.Transition(operational, smi =>
                        smi.CanRadiate(),
                        UpdateRate.SIM_4000ms)
                        .ToggleStatusItem(NotInSpaceStatus, smi => smi.master);
                    operational.DefaultState(operational.radiating).Transition(notInSpace, smi =>
                        !smi.CanRadiate(),
                        UpdateRate.SIM_4000ms);
                    operational.radiating.Transition(operational.tooCold, smi =>
                        smi.master.element.Temperature <= MinimumTemperature,
                        UpdateRate.SIM_200ms)
                        .ToggleStatusItem(RadiatingStatus, smi => smi.master)
                        .Update((smi, dt) => smi.UpdateTemperature(dt), UpdateRate.SIM_200ms);
                    operational.tooCold.Transition(operational.radiating, smi =>
                        smi.master.element.Temperature > MinimumTemperature,
                        UpdateRate.SIM_200ms)
                        .ToggleStatusItem(TooColdStatus, smi => smi.master);
                }
            }
            public class Instance : GameStateMachine<States, Instance, RadiateHeat, object>.GameInstance
            {
                public Instance(RadiateHeat master) : base(master) { }

                public void UpdateTemperature(float dt)
                {
                    float temp = master.element.Temperature;
                    master.CurrentCooling = RadiativeHeat(temp);
                    if (master.CurrentCooling > 0f)
                    {
                        GameComps.StructureTemperatures.ProduceEnergy(master.structureTemperature,
                            -master.CurrentCooling * dt / 1000f, "Radiated", dt);
                    }
                    UpdateStatusItem();
                }

                public float RadiativeHeat(float temp)
                {
                    return StefanBoltzmannConstant
                        * (Mathf.Pow(temp, 4f) * master.Emissivity * master.SurfaceArea);
                }

                protected void UpdateStatusItem()
                {
                    master.selectable.ToggleStatusItem(RadiatingStatus, true, master);
                }

                public bool CanRadiate()
                {
                    // Check whether in space
                    foreach (int cell in master.building.PlacementCells)
                    {
                        if (Grid.Objects[cell, (int)ObjectLayer.Backwall] != null
                           && !(master.building.Def.ObjectLayer == ObjectLayer.Backwall
                           && Grid.Element[cell].IsVacuum))
                            return false;
                    }
                    return true;
                }
            }
        }

        public static class Absorb
        {
            public class States : GameStateMachine<States, Instance, RadiateHeat>
            {
                public class OperationalStates : State
                {
                    public State absorbing;
                    public State tooHot;
                }
                public State notInSunlight;
                public OperationalStates operational;

                public override void InitializeStates(out BaseState default_state)
                {
                    default_state = notInSunlight;
                    notInSunlight.Transition(operational, smi =>
                        smi.CheckInSunlight(),
                        UpdateRate.SIM_1000ms)
                        .ToggleStatusItem(NotInSunlightStatus, smi => smi.master);
                    operational.DefaultState(operational.absorbing).Transition(notInSunlight, smi =>
                        !smi.CheckInSunlight(),
                        UpdateRate.SIM_1000ms);
                    operational.absorbing.Transition(operational.tooHot, smi =>
                        smi.master.element.Temperature >= Options.Instance.MaximumTemperature,
                        UpdateRate.SIM_200ms)
                        .ToggleStatusItem(AbsorbingStatus, smi => smi.master)
                        .Update((smi, dt) => smi.UpdateTemperature(dt), UpdateRate.SIM_200ms);
                    operational.tooHot.Transition(operational.absorbing, smi =>
                        smi.master.element.Temperature < Options.Instance.MaximumTemperature,
                        UpdateRate.SIM_200ms)
                        .ToggleStatusItem(TooHotStatus, smi => smi.master);
                }
            }
            public class Instance : GameStateMachine<States, Instance, RadiateHeat, object>.GameInstance
            {
                protected readonly WorldContainer world;
                protected uint exposedToSunlight;

                public Instance(RadiateHeat master) : base(master)
                {
                    world = master.GetMyWorld();
                }

                public void UpdateTemperature(float dt)
                {
                    master.CurrentHeating = SunlightHeat();
                    if (master.CurrentHeating > 0f)
                    {
                        GameComps.StructureTemperatures.ProduceEnergy(master.structureTemperature,
                            master.CurrentHeating * dt / 1000f, "Absorbed", dt);
                    }
                    UpdateStatusItem();
                }

                public float SunlightHeat()
                {
                    float sunlightIntensity = world ?
                        world.currentSunlightIntensity : Game.Instance.currentFallbackSunlightIntensity;
                    float heat = (float)exposedToSunlight / (float)byte.MaxValue
                        * sunlightIntensity / Options.Instance.SunLightEfficiency;
                    return heat * master.Emissivity;
                }

                protected void UpdateStatusItem()
                {
                    master.selectable.ToggleStatusItem(AbsorbingStatus, true, master);
                }

                public bool CheckInSunlight()
                {
                    bool result = false;
                    exposedToSunlight = 0;
                    byte cellExposedToSunLight;
                    // Check whether in space
                    foreach (int cell in master.building.PlacementCells)
                    {
                        cellExposedToSunLight = Grid.ExposedToSunlight[cell];
                        if (cellExposedToSunLight >= Sim.ClearSkyGridValue)
                        {
                            result = true;
                            exposedToSunlight += cellExposedToSunLight;
                        }
                    }
                    return result;
                }
            }
        }
    }
}