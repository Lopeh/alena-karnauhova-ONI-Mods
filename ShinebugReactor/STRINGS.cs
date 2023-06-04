using GameStrings = STRINGS;
using static STRINGS.UI;

namespace ShinebugReactor
{
    public static class STRINGS
    {
        public static class BUILDINGS
        {
            public static class PREFABS
            {
                public static class SHINEBUGREACTOR
                {
                    public static LocString NAME = FormatAsLink("Shinebug Reactor", ShinebugReactorConfig.ID);
                    public static LocString DESC = $"When eggs enter the reactor, they are stored inside unti they hatch. The newly hatched {FormatAsLink("shine bugs", nameof(GameStrings.CREATURES.SPECIES.LIGHTBUG))} are then enslaved for your gain.";
                    public static LocString EFFECT = $"Receives {FormatAsLink("shine bug", nameof(GameStrings.CREATURES.SPECIES.LIGHTBUG))} eggs and generates {FormatAsLink("Power", nameof(GameStrings.CODEX.POWER))} and {FormatAsLink("Radbolts", nameof(GameStrings.CODEX.RADIATION))} using their light.";
                    public static LocString LOGIC_PORT_FIRE = "Do not generate/emit Radbolts";
                    public static LocString LOGIC_PORT_FIRE_ACTIVE = $"{FormatAsAutomationState("Green Signal", AutomationState.Active)}: generate and emit Radbolts";
                    public static LocString LOGIC_PORT_FIRE_INACTIVE = $"{FormatAsAutomationState("Red Signal", AutomationState.Standby)}: do not generate and emit Radbolts";
                }
            }
        }

        public static class BUILDING
        {
            public static class STATUSITEMS
            {
                public static class SHINEBUGREACTORCREATURES
                {
                    public static LocString NAME = "Creatures: {0}";
                    public static LocString TOOLTIP = "This reactor contains {0} creatures";
                }
                public static class SHINEBUGREACTOREGGS
                {
                    public static LocString NAME = "Eggs: {0}";
                    public static LocString TOOLTIP = "This reactor contains {0} eggs";
                }
                public static class SHINEBUGREACTORWATTAGE
                {
                    public static LocString NAME = "Current Wattage: {0}";
                    public static LocString TOOLTIP = "This reactor is generating {0} of " + FormatAsLink("Power", nameof(GameStrings.CODEX.POWER));
                }
                public static class SHINEBUGREACTORHEP
                {
                    public static LocString NAME = "Radbolt production: {0}/cycle";
                    public static LocString TOOLTIP = "This reactor is generating {0}{1} per cycle";
                }
                public static class SHINEBUGREACTORNOHEPPRODUCTIONWARNING
                {
                    public static LocString NAME = "No radbolt production";
                    public static LocString TOOLTIP = "This reactor is not generating radbolts due to insufficient power generation";
                }
                public static class SHINEBUGREACTORHEPPRODUCTIONDISABLED
                {
                    public static LocString NAME = "Radbolt production disabled";
                    public static LocString TOOLTIP = "Radbolt production is disabled by automation";
                }
            }
        }

        public static class UI
        {
            public static class BUILDINGEFFECTS
            {
                public static class TOOLTIPS
                {
                    public static LocString SHINEBUGREACTORREQUIRESPOWER = "Should generate at least " + FormatAsNegativeRate("{0}") + " of power to produce radbolts";
                    public static LocString SHINEBUGREACTORHEATGENERATED = "Generates " + FormatAsPositiveRate("{0}") + " per second while producing radbolts";
                }
            }
        }

        public static class OPTIONS
        {
            public static class REPRODUCTIONMODE
            {
                public static LocString NAME = "Reproduction mode";
                public static LocString DESC = "Reproduction options for creatures stored in the reactor.";
                public static class IMMORTALITY
                {
                    public static LocString NAME = "Immortality";
                    public static LocString DESC = "Creature in the reactor will neither die nor reproduce.";
                }
                public static class REPRODUCTION
                {
                    public static LocString NAME = "Reproduction";
                    public static LocString DESC = "Creature in the reactor will lay an egg before it dies.";
                }
                public static class FINALDEATH
                {
                    public static LocString NAME = "Final death";
                    public static LocString DESC = "Creature in the reactor will die without reproducing.";
                }
            }
            public static class DROPHATCHED
            {
                public static LocString NAME = "Drop hatched creatures";
                public static LocString DESC = "Reactor drops not only eggs but also hatched creatures on filter change, if enabled.";
            }
            public static class MAXPOWEROUTPUT
            {
                public static LocString NAME = "Max power output (W)";
                public static LocString DESC = "Reactor cannot produce more than this value.";
            }
            public static class FIXINCUBATORPRIORITY
            {
                public static LocString NAME = "Fix incubator and cracker priority";
                public static LocString DESC = "By default, eggs can be delivered to an incubator from a storage with any priority. It means that if item removal is allowed on the reactor, eggs will be transferred from it to lower priority incubators. Enable the option to fix this behavior.";
            }
            public static class POWERGENERATIONMODE
            {
                public static LocString NAME = "Power generation mode";
                public static LocString DESC = "Determines how the amount of power per creature is calculated.";
                public static class SOLARPANEL
                {
                    public static LocString NAME = "Solar panel";
                    public static LocString DESC = "In this mode power is generated similar to solar panels, this means ~25 W for usual shine bug.";
                }
                public static class RATIO
                {
                    public static LocString NAME = "Ratio";
                    public static LocString DESC = "In this mode power per creature = max power output / max creature limit, for any creature that emits light.";
                }
            }
        }
    }
}
