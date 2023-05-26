using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PeterHan.PLib.Options;
using static Meteor_Seasons.STRINGS.OPTIONS;

namespace Meteor_Seasons
{
    [Serializable]
    public class Options : SingletonOptions<Options>, IOptions
    {
        [Serializable]
        public class ShowNotificationsCategory
        {
            [Option("STRINGS.OPTIONS.SHOWNOTIFICATIONS.ALWAYSSHOWACTIVESEASON.NAME", "STRINGS.OPTIONS.SHOWNOTIFICATIONS.ALWAYSSHOWACTIVESEASON.DESC")]
            [JsonProperty]
            public bool AlwaysShowActiveSeason { get; set; } = true;

            /*[Option("STRINGS.OPTIONS.SHOWNOTIFICATIONS.SHOWONLOAD.NAME", "STRINGS.OPTIONS.SHOWNOTIFICATIONS.SHOWONLOAD.DESC")]
            [JsonProperty]
            public bool ShowOnLoad { get; set; } = true;

            [Option("STRINGS.OPTIONS.SHOWNOTIFICATIONS.SHOWONNEWDAY.NAME", "STRINGS.OPTIONS.SHOWNOTIFICATIONS.SHOWONNEWDAY.DESC")]
            [JsonProperty]
            public bool ShowOnNewDay { get; set; } = true;*/

            [Option("STRINGS.OPTIONS.SHOWNOTIFICATIONS.SHOWSEASONCHANGE.NAME", "STRINGS.OPTIONS.SHOWNOTIFICATIONS.SHOWSEASONCHANGE.DESC")]
            [JsonProperty]
            public bool ShowSeasonChange { get; set; } = true;

            [Option("STRINGS.OPTIONS.SHOWNOTIFICATIONS.SHOWBOMBARDMENTSTART.NAME", "STRINGS.OPTIONS.SHOWNOTIFICATIONS.SHOWBOMBARDMENTSTART.DESC")]
            [JsonProperty]
            public bool ShowBombardmentStart { get; set; } = true;
        }

        [Serializable]
        public class GeneralCategory
        {
            [Option("STRINGS.OPTIONS.GENERAL.SHOWBEFORESURFACEBREACH.NAME", "STRINGS.OPTIONS.GENERAL.SHOWBEFORESURFACEBREACH.DESC")]
            [JsonProperty]
            public bool ShowBeforeSurfaceBreach { get; set; } = false;
        }

        [Option("STRINGS.OPTIONS.SHOWNOTIFICATIONS.NAME", category: "STRINGS.OPTIONS.SHOWNOTIFICATIONS.NAME")]
        [JsonProperty]
        public ShowNotificationsCategory ShowNotifications { get; set; } = new ShowNotificationsCategory();

        [Option("STRINGS.OPTIONS.GENERAL.NAME", category: "STRINGS.OPTIONS.GENERAL.NAME")]
        [JsonProperty]
        public GeneralCategory General { get; set; } = new GeneralCategory();

        public IEnumerable<IOptionsEntry> CreateOptions() => null;

        public void OnOptionsChanged()
        {
            instance = POptions.ReadSettings<Options>() ?? new Options();
        }
    }
}
