using ICities;

namespace ModTools
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private static ModToolsManager manager;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            if (manager != null)
            {
                return;
            }
            manager = new ModToolsManager();
            SimulationManager.RegisterSimulationManager(manager);
        }

        public override void OnLevelUnloading()
        {
            ModToolsBootstrap.initialized = false;
            ModToolsBootstrap.inMainMenu = true;
        }
    }
}