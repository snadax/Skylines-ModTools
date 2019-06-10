using System;
using System.Reflection;
using ColossalFramework;
using UnityEngine;

namespace ModTools.Explorer
{
    public static class Plopper
    {
        private static PrefabInfo ploppedPrefab;

        public static void Reset() => ploppedPrefab = null;

        public static void Update()
        {
            if (ploppedPrefab == null)
            {
                return;
            }
            ToolManager toolManager = Singleton<ToolManager>.instance;
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
            ToolBase currentTool = toolManager.m_properties.CurrentTool;
            if (currentTool == null)
            {
                return;
            }
            if (currentTool is BuildingTool || currentTool is NetTool || currentTool is TreeTool
                || currentTool is PropTool)
            {
                FieldInfo prefabField = currentTool.GetType()
                    .GetField("m_prefab", BindingFlags.Instance | BindingFlags.Public);
                if (prefabField != null)
                {
                    object prefab = prefabField.GetValue(currentTool);
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
            ToolBase currentTool = Singleton<ToolManager>.instance.m_properties.CurrentTool;
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
                BuildingTool buildingTool = ToolsModifierControl.GetTool<BuildingTool>();
                if (buildingTool == null)
                {
                    Log.Warning("BuildingTool not found!");
                    return;
                }
                Singleton<ToolManager>.instance.m_properties.CurrentTool = buildingTool;
                ploppedPrefab = buildingTool.m_prefab = buildingInfo;
                buildingTool.m_relocate = 0;
            }
            else if (prefabInfo is NetInfo netInfo)
            {
                NetTool netTool = ToolsModifierControl.GetTool<NetTool>();
                if (netTool == null)
                {
                    Log.Warning("NetTool not found!");
                    return;
                }
                Singleton<ToolManager>.instance.m_properties.CurrentTool = netTool;
                ploppedPrefab = netTool.m_prefab = netInfo;
            }
            else if (prefabInfo is PropInfo propInfo)
            {
                PropTool propTool = ToolsModifierControl.GetTool<PropTool>();
                if (propTool == null)
                {
                    Log.Warning("PropTool not found!");
                    return;
                }
                Singleton<ToolManager>.instance.m_properties.CurrentTool = propTool;
                ploppedPrefab = propTool.m_prefab = propInfo;
            }
            else if (prefabInfo is TreeInfo treeInfo)
            {
                TreeTool treeTool = ToolsModifierControl.GetTool<TreeTool>();
                if (treeTool == null)
                {
                    Log.Warning("TreeTool not found!");
                    return;
                }
                Singleton<ToolManager>.instance.m_properties.CurrentTool = treeTool;
                ploppedPrefab = treeTool.m_prefab = treeInfo;
            }
        }
    }
}