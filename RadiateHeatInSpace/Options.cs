using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace RadiateHeatInSpace
{
    [Serializable] [ConfigFile(IndentOutput: true)] [RestartRequired]
    public class Options : SingletonOptions<Options>
    {
        [JsonProperty]
        [Option("STRINGS.OPTIONS.SUNLIGHTHEAT.NAME", "STRINGS.OPTIONS.SUNLIGHTHEAT.DESC",
            "STRINGS.OPTIONS.CATEGORIES.SUNRADIATION")]
        public bool SunlightHeat { get; set; } = false;
        [JsonProperty]
        [Option("STRINGS.OPTIONS.SUNLIGHTEFFICIENCY.NAME", "STRINGS.OPTIONS.SUNLIGHTEFFICIENCY.DESC",
            "STRINGS.OPTIONS.CATEGORIES.SUNRADIATION")]
        [Limit(1, 300)]
        public int SunLightEfficiency { get; set; } = 93;
        [JsonProperty]
        [Option("STRINGS.OPTIONS.MAXTEMPERATURE.NAME", "STRINGS.OPTIONS.MAXTEMPERATURE.DESC",
            "STRINGS.OPTIONS.CATEGORIES.SUNRADIATION")]
        [Limit(274f, 5274f)]
        public float MaximumTemperature { get; set; } = TUNING.BUILDINGS.OVERHEAT_TEMPERATURES.HIGH_4 - 1f;
        [JsonProperty]
        [Option("Radiative buildings")]
        public Dictionary<string, float> RadiativeBuildings { get; set; } = new Dictionary<string, float>
        {
            { RadiativeTileConfig.ID, 0.77f },
            { AutoMinerConfig.ID, 0.2f },
            { BatteryConfig.ID, 0.8f },
            { BatteryMediumConfig.ID, 0.8f },
            { BatterySmartConfig.ID, 0.7f },
            { PowerTransformerSmallConfig.ID, 0.3f },
            { PowerTransformerConfig.ID, 0.2f },
            { SolidTransferArmConfig.ID, 0.6f },
            { ObjectDispenserConfig.ID, 0.2f },
            { StorageLockerSmartConfig.ID, 0.3f },
            { SolidConduitInboxConfig.ID, 0.4f },
            { CheckpointConfig.ID, 0.1f },
            { GantryConfig.ID, 0.1f },
            //{ SolarPanelConfig.ID, 0.1f },
            { CeilingLightConfig.ID, 0.5f },
            { FloorLampConfig.ID, 0.3f },
            // New stuff
            { MissionControlConfig.ID, 0.2f },
            { MissileLauncherConfig.ID, 0.2f },
            { ContactConductivePipeBridgeConfig.ID, 0.5f },
            // Spaced Out!
            //{ BaseModularLaunchpadPortConfig.LinkTag.Name, 0.3f },
            { ModularLaunchpadPortGasConfig.ID, 0.3f },
            { ModularLaunchpadPortGasUnloaderConfig.ID, 0.3f },
            { ModularLaunchpadPortLiquidConfig.ID, 0.3f },
            { ModularLaunchpadPortLiquidUnloaderConfig.ID, 0.3f },
            { ModularLaunchpadPortSolidConfig.ID, 0.3f },
            { ModularLaunchpadPortSolidUnloaderConfig.ID, 0.3f },
            { RailGunConfig.ID, 0.1f },
            { LandingBeaconConfig.ID, 0.2f },
            { HEPBatteryConfig.ID, 0.4f },
            { HighEnergyParticleSpawnerConfig.ID, 0.7f },
            // Mod
            { "ThermalInterfacePlate", 0.5f },
        };
    }
}