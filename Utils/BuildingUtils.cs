using System;

namespace Utils
{
    public static class BuildingUtils
    {
        public static void AddBuildingToTech(string buildingID, string tech)
        {
            Db.Get().Techs.Get(tech).unlockedItemIDs.Add(buildingID);
        }
    }
}
