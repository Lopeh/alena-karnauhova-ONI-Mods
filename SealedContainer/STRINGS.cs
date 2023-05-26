using static STRINGS.UI;
using GameStrings = STRINGS;

namespace SealedContainer
{
    public static class STRINGS
    {
        public static class BUILDINGS
        {
            public static class PREFABS
            {
                public static class SEALEDCONTAINER
                {
                    public static LocString NAME = FormatAsLink("Sealed Container", SealedContainerConfig.ID);
                    public static LocString DESC = $"Nicely sealed container. Its {FormatAsLink("contents", "ELEMENTSSOLID")} will stay forever!";
                    public static LocString EFFECT = $"Prevents its {FormatAsLink("contents", "ELEMENTSSOLID")} from sublimating.";
                }
                public static class INSULATEDCONTAINER
                {
                    public static LocString NAME = FormatAsLink("Insulated Container", InsulatedContainerConfig.ID);
                    public static LocString DESC = $"This container is not only sealed but also insulated. Force all those {FormatAsLink("DTUs", nameof(GameStrings.CODEX.HEAT))} to stay where they are!";
                    public static LocString EFFECT = $"Prevents sublimation and heat exchange of its {FormatAsLink("contents", "ELEMENTSSOLID")}.";
                }
            }
        }

        public static class OPTIONS
        {
            public static class CAPACITY
            {
                public static LocString NAME = "Capacity (kg)";
                public static LocString DESC = "Determines max capacity of the container.";
            }
            public static class REQUIRESUPERINSULATOR
            {
                public static LocString NAME = "Insulated Container requires SuperInsulator";
                public static LocString DESC = "Only Insulation can be used for Insulated Container construction if enabled.";
            }
        }
    }
}