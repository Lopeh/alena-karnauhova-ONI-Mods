using System;
using static GameComps;

namespace RadiateHeatInSpace
{
    using static STRINGS.BUILDING.STATUSITEMS;

    //[SerializationConfig(MemberSerialization.OptIn)]
    [SkipSaveFileSerialization]
    public class RadiateHeat : KMonoBehaviour
    {
        public const float StefanBoltzmannConstant = 5.67e-8f;
        public const float MinimumTemperature = 3f;
        #region StatusItems
        public const string StatusItemPrefix = "BUILDING";
        public static readonly StatusItem NotInSpaceStatus
            = new StatusItem(nameof(RADIATEHEAT_NOTINSPACE), StatusItemPrefix,
            string.Empty, StatusItem.IconType.Info, NotificationType.Neutral,
            false, OverlayModes.HeatFlow.ID);
        public static readonly StatusItem TooColdStatus
            = new StatusItem(nameof(RADIATEHEAT_TOOCOLD), StatusItemPrefix,
            string.Empty, StatusItem.IconType.Info, NotificationType.Neutral,
            false, OverlayModes.HeatFlow.ID);
        public static readonly StatusItem RadiatingStatus
            = new StatusItem(nameof(RADIATEHEAT_RADIATING), StatusItemPrefix,
            string.Empty, StatusItem.IconType.Info, NotificationType.Neutral,
            false, OverlayModes.HeatFlow.ID,
            resolve_string_callback: ((str, data) =>
            {
                float value = ((RadiateHeat)data).CurrentCooling;
                str = string.Format(str, GameUtil.GetFormattedHeatEnergyRate(value));
                return str;
            }));
        public static readonly StatusItem NotInSunlightStatus
            = new StatusItem(nameof(RADIATEHEAT_NOTINSUNLIGHT), StatusItemPrefix,
            string.Empty, StatusItem.IconType.Info, NotificationType.Neutral,
            false, OverlayModes.HeatFlow.ID);
        public static readonly StatusItem TooHotStatus
            = new StatusItem(nameof(RADIATEHEAT_TOOHOT), StatusItemPrefix,
            string.Empty, StatusItem.IconType.Info, NotificationType.Neutral,
            false, OverlayModes.HeatFlow.ID);
        public static readonly StatusItem AbsorbingStatus
            = new StatusItem(nameof(RADIATEHEAT_ABSORBING), StatusItemPrefix,
            string.Empty, StatusItem.IconType.Info, NotificationType.Neutral,
            false, OverlayModes.HeatFlow.ID,
            resolve_string_callback: ((str, data) =>
            {
                float value = ((RadiateHeat)data).CurrentHeating;
                str = string.Format(str, GameUtil.GetFormattedHeatEnergyRate(value));
                return str;
            }));
        #endregion
        public float Emissivity = 0.9f;
        public float SurfaceArea = 1f;
        #region Components
        [MyCmpGet]
        protected readonly BuildingComplete building;
        [MyCmpGet]
        protected readonly PrimaryElement element;
        #endregion
        protected HandleVector<int>.Handle structureTemperature;
        protected Radiate.Instance RadiateSMI;
        protected Absorb.Instance AbsorbSMI;

        public float CurrentCooling, CurrentHeating;
        
        public static void AddStatusItemsToDatabase(Database.BuildingStatusItems statusItemsList)
        {
            statusItemsList.Add(NotInSpaceStatus);
            statusItemsList.Add(TooColdStatus);
            statusItemsList.Add(RadiatingStatus);
            statusItemsList.Add(NotInSunlightStatus);
            statusItemsList.Add(TooHotStatus);
            statusItemsList.Add(AbsorbingStatus);
        }

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
            BuildingDef def = building.Def;
            //if (!def) return;
            SurfaceArea = def.HeightInCells * def.WidthInCells;
        }

        protected override void OnSpawn()
        {
            base.OnSpawn();

            structureTemperature = StructureTemperatures.GetHandle(gameObject);
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
                        .ToggleStatusItem(NotInSpaceStatus, GetMaster);
                    operational.DefaultState(operational.radiating).Transition(notInSpace, smi =>
                        !smi.CanRadiate(),
                        UpdateRate.SIM_4000ms);
                    operational.radiating.Transition(operational.tooCold, smi =>
                        smi.master.element.Temperature <= MinimumTemperature,
                        UpdateRate.SIM_200ms)
                        .ToggleStatusItem(RadiatingStatus, GetMaster)
                        .Update((smi, dt) => smi.UpdateTemperature(dt), UpdateRate.SIM_200ms);
                    operational.tooCold.Transition(operational.radiating, smi =>
                        smi.master.element.Temperature > MinimumTemperature,
                        UpdateRate.SIM_200ms)
                        .ToggleStatusItem(TooColdStatus, GetMaster);
                }
                protected static readonly Func<Radiate.Instance, object> GetMaster = (smi) => smi.master;
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
                        StructureTemperatures.ProduceEnergy(master.structureTemperature,
                            -master.CurrentCooling * dt / 1000f, OPERATINGENERGY.RADIATED, dt);
                    }
                }

                public float RadiativeHeat(float temp)
                {
                    return StefanBoltzmannConstant * ((temp * temp * temp * temp)
                        * master.Emissivity * master.SurfaceArea);
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
                        .ToggleStatusItem(NotInSunlightStatus, GetMaster);
                    operational.DefaultState(operational.absorbing).Transition(notInSunlight, smi =>
                        !smi.CheckInSunlight(),
                        UpdateRate.SIM_1000ms);
                    operational.absorbing.Transition(operational.tooHot, smi =>
                        smi.master.element.Temperature >= Options.Instance.MaximumTemperature,
                        UpdateRate.SIM_200ms)
                        .ToggleStatusItem(AbsorbingStatus, GetMaster)
                        .Update((smi, dt) => smi.UpdateTemperature(dt), UpdateRate.SIM_200ms);
                    operational.tooHot.Transition(operational.absorbing, smi =>
                        smi.master.element.Temperature < Options.Instance.MaximumTemperature,
                        UpdateRate.SIM_200ms)
                        .ToggleStatusItem(TooHotStatus, GetMaster);
                }
                protected static readonly Func<Absorb.Instance, object> GetMaster = (smi) => smi.master;
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
                        StructureTemperatures.ProduceEnergy(master.structureTemperature,
                            master.CurrentHeating * dt / 1000f, OPERATINGENERGY.ABSORBED, dt);
                    }
                }

                public float SunlightHeat()
                {
                    float sunlightIntensity = world ?
                        world.currentSunlightIntensity : Game.Instance.currentFallbackSunlightIntensity;
                    float heat = (float)exposedToSunlight / (float)byte.MaxValue
                        * sunlightIntensity / Options.Instance.SunLightEfficiency;
                    return heat * master.Emissivity;
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