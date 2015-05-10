using ColossalFramework.Math;
using SkylinesOverwatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gurkenplayer
{
    public static class RoadSyncHelper
    {
        //Test
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
        public static void CreateBuilding(ushort id, Building building)
        {
            BuildingManager.instance.CreateBuilding(out id, ref SimulationManager.instance.m_randomizer, building.Info, building.m_position, building.m_angle, building.Length, building.m_buildIndex);
        }
        public static void CloneNode(NetNode node)
        {
            Log.Message("BuildRoad enter");
            int maxSegments = 100;
            bool test = false;
            bool visualize = false;
            bool autoFix = true;
            bool needMoney = false;
            bool invert = false;
            bool switchDir = false;
            ushort relocateBuildingID = 0;
            ushort firstNode;
            ushort lastNode;
            ushort startNode;
            ushort endNode;
            ushort segment;
            int cost;
            int productionRate;
            Log.Message("BuildRoad fields inied");
            NetTool tool = new NetTool();

            NetInfo netInfo = node.Info;
            float startHeight = NetSegment.SampleTerrainHeight(netInfo, node.m_position, false);
            float endHeight = NetSegment.SampleTerrainHeight(netInfo, node.m_position, false);
            Log.Message("BuildRoad netinfo");
            NetTool.ControlPoint startControlPt = new NetTool.ControlPoint();
            NetTool.ControlPoint endControlPt = new NetTool.ControlPoint();
            Log.Message("BuildRoad netcontrol set");
            node.m_position.y = startHeight;
            startControlPt.m_position = node.m_position;
            node.m_position.y = endHeight;
            endControlPt.m_position = node.m_position;
            Log.Message("BuildRoad creating node 1");

            NetTool.CreateNode(netInfo, startControlPt, startControlPt, startControlPt, NetTool.m_nodePositionsSimulation,
                0, false, false, false, false, false, false, (ushort)0, out startNode, out segment, out cost, out productionRate);

            // CreateNode(out startNode, ref rand, netInfo, new Vector2(startVector.x, startVector.z) , NetSegment.SampleTerrainHeight(netInfo, startVector, false));
            Log.Message("BuildRoad creating node 2");

            NetTool.CreateNode(netInfo, endControlPt, endControlPt, endControlPt, NetTool.m_nodePositionsSimulation,
                0, false, false, false, false, false, false, (ushort)0, out endNode, out segment, out cost, out productionRate);

            // CreateNode(out endNode, ref rand, netInfo, new Vector2(endVector.x, endVector.z), NetSegment.SampleTerrainHeight(netInfo, startVector, false));

            // Array16<NetNode> abc = NetManager.instance.m_nodes; Test

            startControlPt.m_node = startNode;
            endControlPt.m_node = endNode;
            Log.Message("BuildRoad midcontrpt setting");

            NetTool.ControlPoint midControlPt = endControlPt;
            midControlPt.m_position = (startControlPt.m_position + endControlPt.m_position) * 0.5f;
            midControlPt.m_direction = VectorUtils.NormalizeXZ(midControlPt.m_position - startControlPt.m_position);
            endControlPt.m_direction = VectorUtils.NormalizeXZ(endControlPt.m_position - midControlPt.m_position);
            NetTool.CreateNode(netInfo, startControlPt, midControlPt, endControlPt, NetTool.m_nodePositionsSimulation,
                 maxSegments, test, visualize, autoFix, needMoney, invert, switchDir, relocateBuildingID, out firstNode,
                 out lastNode, out segment, out cost, out productionRate);
            Log.Message("BuildRoad road set");
        }
        public static void BuildRoad(Vector3 startVector, Vector3 endVector, uint prefabNumber)
        {
            int maxSegments = 100;
            bool test = false;
            bool visualize = false;
            bool autoFix = true;
            bool needMoney = false;
            bool invert = false;
            bool switchDir = false;
            ushort relocateBuildingID = 0;
            ushort firstNode;
            ushort lastNode;
            ushort startNode;
            ushort endNode;
            ushort segment;
            int cost;
            int productionRate;
            NetTool tool = new NetTool();

            NetInfo netInfo = PrefabCollection<NetInfo>.GetPrefab(prefabNumber);
            float startHeight = NetSegment.SampleTerrainHeight(netInfo, startVector, false);
            float endHeight = NetSegment.SampleTerrainHeight(netInfo, endVector, false);
            NetTool.ControlPoint startControlPt = new NetTool.ControlPoint();
            NetTool.ControlPoint endControlPt = new NetTool.ControlPoint();
            startVector.y = startHeight;
            startControlPt.m_position = startVector;
            endVector.y = endHeight;
            endControlPt.m_position = endVector;

            NetTool.CreateNode(netInfo, startControlPt, startControlPt, startControlPt, NetTool.m_nodePositionsSimulation,
                0, false, false, false, false, false, false, (ushort)0, out startNode, out segment, out cost, out productionRate);


            NetTool.CreateNode(netInfo, endControlPt, endControlPt, endControlPt, NetTool.m_nodePositionsSimulation,
                0, false, false, false, false, false, false, (ushort)0, out endNode, out segment, out cost, out productionRate);

            startControlPt.m_node = startNode;
            endControlPt.m_node = endNode;
            NetTool.ControlPoint midControlPt = endControlPt;
            midControlPt.m_position = (startControlPt.m_position + endControlPt.m_position) * 0.5f;
            midControlPt.m_direction = VectorUtils.NormalizeXZ(midControlPt.m_position - startControlPt.m_position);
            endControlPt.m_direction = VectorUtils.NormalizeXZ(endControlPt.m_position - midControlPt.m_position);
            NetTool.CreateNode(netInfo, startControlPt, midControlPt, endControlPt, NetTool.m_nodePositionsSimulation,
                 maxSegments, test, visualize, autoFix, needMoney, invert, switchDir, relocateBuildingID, out firstNode,
                 out lastNode, out segment, out cost, out productionRate);

        }

        //Test
        public static void AdjustElevation(ushort startNode, Vector3 vector, NetInfo netInfo)
        {
            NetManager nm = NetManager.instance;
            var node = nm.m_nodes.m_buffer[startNode];
            float elevation = NetSegment.SampleTerrainHeight(netInfo, vector, false);
            byte ele = (byte)Mathf.Clamp(Mathf.RoundToInt(Math.Max(node.m_elevation, elevation)), 0, 255);
            float terrain = TerrainManager.instance.SampleRawHeightSmoothWithWater(node.m_position, false, 0f);
            node.m_elevation = ele;
            node.m_position = new Vector3(node.m_position.x, ele + terrain, node.m_position.z);
            if (elevation < 11f)
            {
                node.m_flags |= NetNode.Flags.OnGround;
            }
            else
            {
                node.m_flags &= ~NetNode.Flags.OnGround;
                UpdateSegment(node.m_segment0, elevation);
                UpdateSegment(node.m_segment1, elevation);
                UpdateSegment(node.m_segment2, elevation);
                UpdateSegment(node.m_segment3, elevation);
                UpdateSegment(node.m_segment4, elevation);
                UpdateSegment(node.m_segment5, elevation);
                UpdateSegment(node.m_segment6, elevation);
                UpdateSegment(node.m_segment7, elevation);

            }
            nm.m_nodes.m_buffer[startNode] = node;
            //Singleton<NetManager>.instance.UpdateNode(startNode);
        }
        public static void UpdateSegment(ushort segmentId, float elevation)
        {
            if (segmentId == 0)
            {
                return;
            }
            NetManager nm = NetManager.instance;
            if (elevation > 4)
            {
                var errors = default(ToolBase.ToolErrors);
                nm.m_segments.m_buffer[segmentId].Info = nm.m_segments.m_buffer[segmentId].Info.m_netAI.GetInfo(elevation, 5, false, false, false, false, ref errors);
            }
        }
        public static void CreateNode(out ushort startNode, ref Randomizer rand, NetInfo netInfo, Vector2 oldPos, float elevation)
        {
            var pos = new Vector3(oldPos.x, 0, oldPos.y);
            pos.y = TerrainManager.instance.SampleRawHeightSmoothWithWater(pos, false, 0f);
            var nm = NetManager.instance;
            nm.CreateNode(out startNode, ref rand, netInfo, pos, SimulationManager.instance.m_currentBuildIndex);
            SimulationManager.instance.m_currentBuildIndex += 1u;
        }

    }
}
