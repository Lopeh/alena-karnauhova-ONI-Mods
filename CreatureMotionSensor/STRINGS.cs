using static STRINGS.UI;
using GameStrings = STRINGS;

namespace Creature_Motion_Sensor
{
    public static class STRINGS
    {
        public static class BUILDINGS
        {
            public static class PREFABS
            {
                public static class LOGICCREATURESENSOR
                {
                    public static LocString NAME = FormatAsLink("Creature Motion Sensor", LogicCreatureSensorConfig.ID);
                    public static LocString DESC = $"Motion sensors can be used for special cases in ranching by sensing when critters are nearby.";
                    public static LocString EFFECT = $"Sends a {FormatAsAutomationState("Green Signal", AutomationState.Active)} or a {FormatAsAutomationState("Red Signal", AutomationState.Standby)} based on whether a critter is in the sensor's range.";
                    public static LocString LOGIC_PORT = "Creature Motion Sensor";
                    public static LocString LOGIC_PORT_ACTIVE = $"Sends a {FormatAsAutomationState("Green Signal", AutomationState.Active)} while a critter is in the sensor's tile range";
                    public static LocString LOGIC_PORT_INACTIVE = $"Otherwise, sends a {FormatAsAutomationState("Red Signal", AutomationState.Standby)}";
                }
            }
        }
        public static class UI
        {
            public static class UISIDESCREENS
            {
                public static class LOGICCREATURESENSORSIDESCREEN
                {
                    public static LocString TOOLTIP = "Will send a " + FormatAsAutomationState("Green Signal", AutomationState.Active) + " if there is a critter withing <b>{0}</b> meters.";
                }
            }
        }
    }
}