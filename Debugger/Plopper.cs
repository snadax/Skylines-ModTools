using System;
using System.Reflection;
using ColossalFramework;
using UnityEngine;

namespace ModTools.Explorer
{
    internal static class Plopper
    {
        private static PrefabInfo ploppedPrefab;

        public static void Reset() => ploppedPrefab = null;

        public static void Update()
        {
            if (ploppedPrefab == null)
            {
                return;
            }
            var toolManager = Singleton<ToolManager>.instance;
            if (toolManager?.m_properties == null)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Singleton<ToolManager>.instance.m_properties.CurrentTool = ToolsModifierControl.GetTool<DefaultTool>();
                ploppedPrefab = null;
                return;
            }
            var currentTool = toolManager.m_properties.CurrentTool;
            if (currentTool == null)
            {
                return;
            }
            if (currentTool is BuildingTool || currentTool is NetTool || currentTool is TreeTool
                || currentTool is PropTool)
            {
                var prefabField = currentTool.GetType()
                    .GetField("m_prefab", BindingFlags.Instance | BindingFlags.Public);
                if (prefabField != null)
                {
                    var prefab = prefabField.GetValue(currentTool);
                    if ((PrefabInfo)prefab != ploppedPrefab)
                    {
                        ploppedPrefab = null;
                    }
                }
                else
                {
                    ploppedPrefab = null;
                }
            }
            else
            {
                ploppedPrefab = null;
            }
        }

        public static void StartPlopping(PrefabInfo prefabInfo)
        {
            var currentTool = Singleton<ToolManager>.instance.m_properties.CurrentTool;
            if (currentTool == null)
            {
                return;
            }

            Type toolType;
            if (prefabInfo is BuildingInfo)
            {
                toolType = typeof(BuildingTool);
            }
            else if (prefabInfo is NetInfo)
            {
                toolType = typeof(NetTool);
            }
            else if (prefabInfo is PropInfo)
            {
                toolType = typeof(PropTool);
            }
            else if (prefabInfo is TreeInfo)
            {
                toolType = typeof(TreeTool);
            }
            else
            {
                toolType = null;
            }

            if (toolType == null || currentTool.GetType() != toolType && !(currentTool is DefaultTool))
            {
                return;
            }
            if (prefabInfo is BuildingInfo buildingInfo)
            {
                var buildingTool = ToolsModifierControl.GetTool<BuildingTool>();
                if (buildingTool == null)
                {
                    Log.Warning("BuildingTool not found!");
                    return;
                }
                Singleton<ToolManager>.instance.m_properties.CurrentTool = buildingTool;
                buildingTool.m_prefab = buildingInfo;
                ploppedPrefab = buildingInfo;
                buildingTool.m_relocate = 0;
            }
            else if (prefabInfo is NetInfo netInfo)
            {
                var netTool = ToolsModifierControl.GetTool<NetTool>();
                if (netTool == null)
                {
                    Log.Warning("NetTool not found!");
                    return;
                }
                Singleton<ToolManager>.instance.m_properties.CurrentTool = netTool;
                netTool.m_prefab = netInfo;
                ploppedPrefab = netInfo;
            }
            else if (prefabInfo is PropInfo propInfo)
            {
                var propTool = ToolsModifierControl.GetTool<PropTool>();
                if (propTool == null)
                {
                    Log.Warning("PropTool not found!");
                    return;
                }
                Singleton<ToolManager>.instance.m_properties.CurrentTool = propTool;
                propTool.m_prefab = propInfo;
                ploppedPrefab = propInfo;
            }
            else if (prefabInfo is TreeInfo treeInfo)
            {
                var treeTool = ToolsModifierControl.GetTool<TreeTool>();
                if (treeTool == null)
                {
                    Log.Warning("TreeTool not found!");
                    return;
                }
                Singleton<ToolManager>.instance.m_properties.CurrentTool = treeTool;
                treeTool.m_prefab = treeInfo;
                ploppedPrefab = treeInfo;
            }
        }
    }
}