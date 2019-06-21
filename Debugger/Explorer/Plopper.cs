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

            var defaultToolActive = currentTool is DefaultTool;

            switch (prefabInfo)
            {
                case BuildingInfo buildingInfo when defaultToolActive || currentTool is BuildingTool:
                    var buildingTool = ToolsModifierControl.GetTool<BuildingTool>();
                    if (buildingTool != null)
                    {
                        buildingTool.m_prefab = buildingInfo;
                        buildingTool.m_relocate = 0;

                        SetCurrentTool(buildingTool);
                    }

                    break;

                case NetInfo netInfo when defaultToolActive || currentTool is NetTool:
                    var netTool = ToolsModifierControl.GetTool<NetTool>();
                    if (netTool != null)
                    {
                        netTool.m_prefab = netInfo;
                        SetCurrentTool(netTool);
                    }

                    break;

                case PropInfo propInfo when defaultToolActive || currentTool is PropTool:
                    var propTool = ToolsModifierControl.GetTool<PropTool>();
                    if (propTool != null)
                    {
                        propTool.m_prefab = propInfo;
                        SetCurrentTool(propTool);
                    }

                    break;

                case TreeInfo treeInfo when defaultToolActive || currentTool is TreeTool:
                    var treeTool = ToolsModifierControl.GetTool<TreeTool>();
                    if (treeTool != null)
                    {
                        ploppedPrefab = treeInfo;
                        SetCurrentTool(treeTool);
                    }

                    break;
            }

            void SetCurrentTool(ToolBase tool)
            {
                Singleton<ToolManager>.instance.m_properties.CurrentTool = tool;
                ploppedPrefab = prefabInfo;
            }
        }
    }
}