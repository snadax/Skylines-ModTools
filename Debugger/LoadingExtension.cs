using System;
using ColossalFramework;
using ICities;

namespace ModTools
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            ModToolsBootstrap.inMainMenu = false;
            ModToolsBootstrap.initialized = false;
            ModToolsBootstrap.Bootstrap();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            CustomPrefabs.Bootstrap();
            var appMode = Singleton<ToolManager>.instance.m_properties.m_mode;
            if (ModTools.Instance.config.extendGamePanels && appMode == ItemClass.Availability.Game)
            {
                ModTools.Instance.gameObject.AddComponent<GamePanelExtender>();
            }
        }

        public override void OnReleased()
        {
            base.OnReleased();
            CustomPrefabs.Revert();
            ModToolsBootstrap.inMainMenu = true;
            ModToolsBootstrap.initialized = false;
        }
    }
}