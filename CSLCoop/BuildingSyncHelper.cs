using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSLCoop
{
    public static class BuildingSyncHelper
    {
        public static Dictionary<ushort, ushort> syncedBuildingDict = new Dictionary<ushort, ushort>();
        public static void CreateBuilding(ushort b)
        {

        }
        public static void He()
        {
            Building b = BuildingManager.instance.m_buildings.m_buffer[4];

        }
        public static void UpdateBuildings(ushort[] newBuildingIDS, Building[] newBuildings)
        {
            for (int i = 0; i < newBuildingIDS.Length; i++)
            {
                CreateBuilding(newBuildingIDS[i], newBuildings[i]);
            }
        }
        public static ushort CreateBuilding(ushort mpID, Building building)
        {
            ConvertionHelper.ConvertToByteArray(building);
            ushort gotID;
            BuildingManager.instance.CreateBuilding(out gotID, ref SimulationManager.instance.m_randomizer, building.Info, building.m_position, building.m_angle, building.Length, building.m_buildIndex);
            if(!syncedBuildingDict.ContainsKey(gotID))
                syncedBuildingDict.Add(gotID, mpID);
            return gotID;
        }
    }
}