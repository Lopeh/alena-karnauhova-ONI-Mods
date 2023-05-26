using static STRINGS.UI;
using GameStrings = STRINGS;

namespace SizeNotIncluded
{
    public static class STRINGS
    {
        public static class OPTIONS
        {
            public static class MAPTYPE
            {
                public static LocString NAME = "Map Type";
                public static LocString DESC = "The type of small map. Description for each contains map size and percentage size compared to default map.";
            }
            public static class SIZENOTINCLUDEDMAPTYPE
            {
                public static class SMALLEST
                {
                    public static LocString NAME = "Smallest";
                    public static LocString DESC = "128x192 size, 25% area";
                }
                public static class SMALL
                {
                    public static LocString NAME = "Small";
                    public static LocString DESC = "160x224 size, 40% area";
                }
                public static class MEDIUM
                {
                    public static LocString NAME = "Medium";
                    public static LocString DESC = "192x288 size, 55% area";
                }
                public static class LARGE
                {
                    public static LocString NAME = "Large";
                    public static LocString DESC = "224x320 size, 75% area";
                }
                public static class DEFAULT
                {
                    public static LocString NAME = "Default";
                    public static LocString DESC = "256x384 size, 100% area";
                }
                public static class SMALLESTTALL
                {
                    public static LocString NAME = "Smallest Tall";
                    public static LocString DESC = "96x224 size, 25% area";
                }
                public static class SMALLTALL
                {
                    public static LocString NAME = "Small Tall";
                    public static LocString DESC = "128x288 size, 40% area";
                }
                public static class MEDIUMTALL
                {
                    public static LocString NAME = "Medium Tall";
                    public static LocString DESC = "160x320 size, 55% area";
                }
                public static class LARGETALL
                {
                    public static LocString NAME = "Large Tall";
                    public static LocString DESC = "192x384 size, 75% area";
                }
                public static class DEFAULTTALL
                {
                    public static LocString NAME = "Default Tall";
                    public static LocString DESC = "224x416 size, 100% area";
                }
            }
            public static class GEYSERCOUNT
            {
                public static LocString NAME = "Generic geyser count";
                public static LocString DESC = "Generic (non-guaranteed) geysers count. Vanilla value is 12.";
            }
        }
    }
}
