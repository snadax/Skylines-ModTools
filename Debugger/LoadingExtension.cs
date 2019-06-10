﻿using ColossalFramework;
using ICities;

namespace ModTools
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            ModToolsBootstrap.Bootstrap();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            CustomPrefabs.Bootstrap();
            ItemClass.Availability appMode = Singleton<ToolManager>.instance.m_properties.m_mode;
            ModTools modTools = ModTools.Instance;
            if (modTools == null)
            {
                UnityEngine.Debug.LogError("ModTools instance wasn't present");
                return;
            }
            if (modTools.config.extendGamePanels && appMode == ItemClass.Availability.Game)
            {
                modTools.gameObject.AddComponent<GamePanelExtender>();
            }
        }

        public override void OnReleased()
        {
            base.OnReleased();
            CustomPrefabs.Revert();
        }
    }
}