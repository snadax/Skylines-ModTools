using System;
using System.Linq;
using ColossalFramework;
using ColossalFramework.Plugins;
using ICities;
using UnityEngine;

namespace ModTools
{

    public class ModToolsBootstrap
    {

        private static GameObject modToolsGameObject;
        private static ModTools modTools;
        public static bool initialized;
        private static bool bootstrapped;
        public static bool inMainMenu = true;

        public static bool IsModToolsActive()
        {
#if DEBUG
            return true;
#else
            var pluginManager = PluginManager.instance;
            var plugins = pluginManager.GetPluginsInfo();

            foreach (var item in plugins)
            {
                var instances = item.GetInstances<IUserMod>();
                if (instances.FirstOrDefault() is Mod)
                {
                    return item.isEnabled;
                }
            }

            return false;
#endif
        }

        public static void Bootstrap()
        {
            if (initialized)
            {
                return;
            }
            try
            {
                if (!bootstrapped)
                {
                    CODebugBase<LogChannel>.verbose = true;
                    CODebugBase<LogChannel>.EnableChannels(LogChannel.All);
                    RedirectionHelper.RedirectCalls(
                        typeof(LoadingWrapper).GetMethod("OnLevelLoaded", new[] { typeof(SimulationManager.UpdateMode) }),
                        typeof(IsolatedFailures).GetMethod("OnLevelLoaded", new[] { typeof(SimulationManager.UpdateMode) }));
                    bootstrapped = true;
                }
                if (inMainMenu)
                {
                    InitModTools(SimulationManager.UpdateMode.Undefined);                    
                }
                initialized = true;
            }
            catch (Exception ex)
            {
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Error, ex.Message);
            }
        }

        public static void InitModTools(SimulationManager.UpdateMode mode)
        {
            if (!IsModToolsActive())
            {
                return;
            }

            if (modToolsGameObject != null)
            {
                return;
            }

            modToolsGameObject = new GameObject("ModTools");
            modTools = modToolsGameObject.AddComponent<ModTools>();
            modTools.Initialize(mode);
        }

    }

    public class Mod : LoadingExtensionBase, IUserMod
    {

        public string Name
        {
            get { ModToolsBootstrap.Bootstrap(); return "ModTools"; }
        }

        public string Description
        {
            get { return "Debugging toolkit for modders"; }
        }

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
