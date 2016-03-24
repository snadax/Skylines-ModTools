using ICities;

namespace ModTools
{
    public class LoadingExtension : LoadingExtensionBase
    {

        public override void OnLevelLoaded(LoadMode mode)
        {
            ModToolsBootstrap.inMainMenu = false;
            ModToolsBootstrap.InitModTools((SimulationManager.UpdateMode)mode);
        }

        public override void OnLevelUnloading()
        {
            ModToolsBootstrap.initialized = false;
            ModToolsBootstrap.inMainMenu = true;
        }
    }
}