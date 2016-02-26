using ICities;

namespace ModTools
{
    public class LoadingExtension : LoadingExtensionBase
    {

        public override void OnLevelLoaded(LoadMode mode)
        {
            ModToolsBootstrap.inMainMenu = false;
            ModToolsBootstrap.InitModTools((SimulationManager.UpdateMode)mode);
            ToolsModifierControl.toolController.m_enableDevUI = true;
        }

        public override void OnLevelUnloading()
        {
            ModToolsBootstrap.initialized = false;
            ModToolsBootstrap.inMainMenu = true;
        }
    }
}