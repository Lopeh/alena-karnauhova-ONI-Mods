using static STRINGS.UI;
using GameStrings = STRINGS;

namespace Meteor_Seasons
{
    public static class STRINGS
    {
        public static class OPTIONS
        {
            public static class SHOWNOTIFICATIONS
            {
                public static LocString NAME = "Show notifications";
                public static class ALWAYSSHOWACTIVESEASON
                {
                    public static LocString NAME = "Always";
                    public static LocString DESC = "Current season is always displayed if active.";
                }
                /*public static class SHOWONLOAD
                {
                    public static LocString NAME = "On load";
                    public static LocString DESC = "Current season is notified after loading.";
                }
                public static class SHOWONNEWDAY
                {
                    public static LocString NAME = "New cycle";
                    public static LocString DESC = "Current season is notified when a new cycle starts.";
                }*/
                public static class SHOWSEASONCHANGE
                {
                    public static LocString NAME = "Season change";
                    public static LocString DESC = "New seasons are notified.";
                }
                public static class SHOWBOMBARDMENTSTART
                {
                    public static LocString NAME = "Meteor shower";
                    public static LocString DESC = "Meteor showers are notified.";
                }
            }
            public static class GENERAL
            {
                public static LocString NAME = "General";
                public static class SHOWBEFORESURFACEBREACH
                {
                    public static LocString NAME = "Show before surface breach";
                    public static LocString DESC = "All notifications will be shown before surface breach. Not recommended.";
                }
            }
        }
        public static class MISC
        {
            public static class NOTIFICATIONS
            {
                public static LocString METEORSEASONACTIVE = "Meteor season is active";
                public static LocString CURRENTSEASON = "Current season: ";
                public static LocString NEWSEASON = "New season: ";
                public static LocString SEASONEND = "Season end: ";
                public static LocString METEORSHOWERS = "Meteor Showers! ";
                public static LocString SHOWERSENDING = "Showers ending: ";
                public static LocString UNKNOWNSEASON = "???";
                public static class METEORSHOWERNONE
                {
                    public static LocString NAME = "No Meteors";
                    public static LocString TOOLTIP = "Nothing to harvest";
                }
                public static class METEORSHOWERIRON
                {
                    public static LocString NAME = "Iron Meteors";
                    public static LocString TOOLTIP = "There will be refined Iron";
                }
                public static class METEORSHOWERCOPPER
                {
                    public static LocString NAME = "Copper Meteors";
                    public static LocString TOOLTIP = "There will be Copper Ore";
                }
                public static class METEORSHOWERGOLD
                {
                    public static LocString NAME = "Gold Meteors";
                    public static LocString TOOLTIP = "There will be Gold Amalgam";
                }
                public static class METEORSHOWERDUST
                {
                    public static LocString NAME = "Dust Meteors";
                    public static LocString TOOLTIP = string.Empty;//"There will be Regolith";
                }
                public static class METEORSHOWERFULLERENE
                {
                    public static LocString NAME = "Fullerene Meteors";
                    public static LocString TOOLTIP = string.Empty;//"There will be Fullerene";
                }
                public static class METEORSHOWERGASSYMOO
                {
                    public static LocString NAME = "Gassy Mooteors";
                    public static LocString TOOLTIP = string.Empty;//"There will be Gassy Moos";
                }
            }
        }
    }
}
