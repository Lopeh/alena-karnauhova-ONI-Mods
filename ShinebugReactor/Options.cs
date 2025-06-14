using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace ShinebugReactor
{
    [Serializable] [RestartRequired]
    public class Options : SingletonOptions<Options>, IOptions
    {
        [JsonProperty]
        [Option("STRINGS.OPTIONS.REPRODUCTIONMODE.NAME", "STRINGS.OPTIONS.REPRODUCTIONMODE.DESC")]
        public ReproductionModeType ReproductionMode { get; set; } = ReproductionModeType.Reproduction;

        [JsonProperty]
        [Option("STRINGS.OPTIONS.DROPHATCHED.NAME", "STRINGS.OPTIONS.DROPHATCHED.DESC")]
        public bool DropHatched { get; set; } = true;

        [JsonProperty]
        [Option("STRINGS.OPTIONS.MAXPOWEROUTPUT.NAME", "STRINGS.OPTIONS.MAXPOWEROUTPUT.DESC")]
        [Limit((int)ShinebugReactorConfig.WattageRequired, 3600)]
        public int MaxPowerOutput { get; set; } = 1200;

        [JsonProperty]
        [Option("STRINGS.OPTIONS.POWERGENERATIONMODE.NAME", "STRINGS.OPTIONS.POWERGENERATIONMODE.DESC")]
        public PowerGenerationModeType PowerGenerationMode { get; set; } = PowerGenerationModeType.SolarPanel;

        [JsonProperty]
        [Option("STRINGS.OPTIONS.STATICLIGHTEMISSION.NAME", "STRINGS.OPTIONS.STATICLIGHTEMISSION.DESC")]
        public bool StaticLightEmission { get; set; } = true;

        [JsonProperty]
        [Option("STRINGS.OPTIONS.FIXINCUBATORPRIORITY.NAME", "STRINGS.OPTIONS.FIXINCUBATORPRIORITY.DESC")]
        public bool FixIncubatorPriority { get; set; } = false;

        public IEnumerable<IOptionsEntry> CreateOptions() => null;

        public void OnOptionsChanged()
        {
            instance = POptions.ReadSettings<Options>() ?? new Options();
        }

        public enum ReproductionModeType : byte
        {
            [Option("STRINGS.OPTIONS.REPRODUCTIONMODE.IMMORTALITY.NAME", "STRINGS.OPTIONS.REPRODUCTIONMODE.IMMORTALITY.DESC")]
            Immortality,
            [Option("STRINGS.OPTIONS.REPRODUCTIONMODE.REPRODUCTION.NAME", "STRINGS.OPTIONS.REPRODUCTIONMODE.REPRODUCTION.DESC")]
            Reproduction,
            [Option("STRINGS.OPTIONS.REPRODUCTIONMODE.FINALDEATH.NAME", "STRINGS.OPTIONS.REPRODUCTIONMODE.FINALDEATH.DESC")]
            FinalDeath,
        }

        public enum PowerGenerationModeType : byte
        {
            [Option("STRINGS.OPTIONS.POWERGENERATIONMODE.SOLARPANEL.NAME", "STRINGS.OPTIONS.POWERGENERATIONMODE.SOLARPANEL.DESC")]
            SolarPanel,
            [Option("STRINGS.OPTIONS.POWERGENERATIONMODE.RATIO.NAME", "STRINGS.OPTIONS.POWERGENERATIONMODE.RATIO.DESC")]
            Ratio,
        }
    }
}