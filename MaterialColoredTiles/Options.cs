using System;
using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace MaterialColoredTilesAndMore
{
    [Serializable] [RestartRequired]
    public class Options : SingletonOptions<Options>
    {
        [JsonProperty]
        [Option("STRINGS.OPTIONS.BRIGHTNESS.NAME", "STRINGS.OPTIONS.BRIGHTNESS.DESC")]
        [Limit(0.75, 1.5)]
        public float Brightness { get; set; } = 1f;

        [JsonProperty]
        [Option("STRINGS.OPTIONS.TILES.NAME", "STRINGS.OPTIONS.TILES.DESC")]
        public bool Tiles { get; set; } = true;

        [JsonProperty]
        [Option("STRINGS.OPTIONS.WALLS.NAME", "STRINGS.OPTIONS.WALLS.DESC")]
        public bool Walls { get; set; }

        [JsonProperty]
        [Option("STRINGS.OPTIONS.DOORS.NAME")]
        public bool Doors { get; set; }

        [JsonProperty]
        [Option("STRINGS.OPTIONS.PIPES.NAME")]
        public bool Pipes { get; set; }

        [JsonProperty]
        [Option("STRINGS.OPTIONS.SCULPTURES.NAME")]
        public bool Sculptures { get; set; }

        [JsonProperty]
        [Option("STRINGS.OPTIONS.MOULDING.NAME")]
        public bool Moulding { get; set; }

        [JsonProperty]
        [Option("STRINGS.OPTIONS.LOGICWIRES.NAME")]
        public bool LogicWires { get; set; }

        [JsonProperty]
        [Option("STRINGS.OPTIONS.FARMTILES.NAME")]
        public bool FarmTiles { get; set; }
    }
}
