using static STRINGS.UI;
using GameStrings = STRINGS;

namespace RadiateHeatInSpace
{
    public static class STRINGS
    {
        public static class BUILDINGS
        {
            public static class PREFABS
            {
                public static class RADIATIVETILE
                {
                    public static LocString NAME = FormatAsLink("Radiative Tile", RadiativeTileConfig.ID);
                    public static LocString DESC = $"Perfect for building a sauna, if the sauna were in space. On second thought, not so perfect for building a sauna.";
                    public static LocString EFFECT = $"A tile that passively radiates {FormatAsLink("heat", nameof(GameStrings.CODEX.HEAT))} to space. More effective the hotter it is.";
                }
            }
        }
        public static class BUILDING
        {
            public static class STATUSITEMS
            {
                public static class RADIATEHEAT_NOTINSPACE
                {
                    public static LocString NAME = "Not in space";
                    public static LocString TOOLTIP = "This building is not fully in space and is not radiating heat.";
                }
                public static class RADIATEHEAT_TOOCOLD
                {
                    public static LocString NAME = "Too cold";
                    public static LocString TOOLTIP = "This building is too cold to radiate heat.";
                }
                public static class RADIATEHEAT_RADIATING
                {
                    public static LocString NAME = "Radiating {0}";
                    public static LocString TOOLTIP = "This building is currently radiating heat at {0}.";
                }
                public static class RADIATEHEAT_NOTINSUNLIGHT
                {
                    public static LocString NAME = "Not in sunlight";
                    public static LocString TOOLTIP = "This building is not in sunlight and is not heated by it.";
                }
                public static class RADIATEHEAT_TOOHOT
                {
                    public static LocString NAME = "Too hot";
                    public static LocString TOOLTIP = "This building is too hot to absorb heat from sun.";
                }
                public static class RADIATEHEAT_ABSORBING
                {
                    public static LocString NAME = "Absorbing {0}";
                    public static LocString TOOLTIP = "This building is currently absorbing heat from sun at {0}";
                }
            }
        }
        public static class OPTIONS
        {
            public static class SUNLIGHTHEAT
            {
                public static LocString NAME = "Sunlight heat";
                public static LocString DESC = "Buildings in sunlight will be heated if enabled.";
            }
            public static class SUNLIGHTEFFICIENCY
            {
                public static LocString NAME = "Sun light efficiency";
                public static LocString DESC = "Lumen per watt ratio of the sun. The more light efficiency, the less heat energy.";
            }
            public static class MAXTEMPERATURE
            {
                public static LocString NAME = "Maximum temperature (K)";
                public static LocString DESC = "Sun cannot heat a building over this temperature.";
            }
            public static class CATEGORIES
            {
                public static LocString SUNRADIATION = "Sun radiation";
            }
        }
    }
}