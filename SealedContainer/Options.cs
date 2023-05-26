using System;
using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace SealedContainer
{
    [Serializable] [RestartRequired]
    public class Options : SingletonOptions<Options>
    {
        [JsonProperty]
        [Option("STRINGS.OPTIONS.CAPACITY.NAME", "STRINGS.OPTIONS.CAPACITY.DESC", Format = "F1")]
        [Limit(1000f, 100000f)]
        public float Capacity { get; set; } = 20000f;

        [JsonProperty]
        [Option("STRINGS.OPTIONS.REQUIRESUPERINSULATOR.NAME", "STRINGS.OPTIONS.REQUIRESUPERINSULATOR.DESC")]
        public bool RequireSuperInsulator { get; set; } = true;
    }
}
