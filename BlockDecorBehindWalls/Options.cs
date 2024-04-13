using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace BlockDecorBehindWalls
{
    [Serializable]
    public class Options : SingletonOptions<Options>, IOptions
    {
        [JsonProperty]
        [Option("STRINGS.OPTIONS.AFFECTHEAVYWIRES.NAME", "STRINGS.OPTIONS.AFFECTHEAVYWIRES.DESC")]
        public bool AffectHeavyWires { get; set; } = true;

        public IEnumerable<IOptionsEntry> CreateOptions() => null;

        public void OnOptionsChanged()
        {
            instance = POptions.ReadSettings<Options>() ?? new Options();
        }
    }
}
