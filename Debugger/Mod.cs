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
        private static bool bootstrapped;

        public static bool IsModToolsActive()
        {

#if MODTOOLS_DEBUG
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
            if (bootstrapped)
            {
                return;
            }
            try
            {
                CODebugBase<LogChannel>.verbose = true;
                CODebugBase<LogChannel>.EnableChannels(LogChannel.All);

                InitModTools(SimulationManager.UpdateMode.Undefined);

                RedirectionHelper.RedirectCalls(
                    typeof(LoadingWrapper).GetMethod("OnLevelLoaded", new[] { typeof(SimulationManager.UpdateMode) }),
                    typeof(ModToolsBootstrap).GetMethod("OnLevelLoaded", new[] { typeof(SimulationManager.UpdateMode) }));
                bootstrapped = true;
            }
            catch (Exception ex)
            {
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Error, ex.Message);
            }
        }

        private static void InitModTools(SimulationManager.UpdateMode mode)
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

        public void OnLevelLoaded(SimulationManager.UpdateMode mode)
        {
            InitModTools(mode);
            IsolatedFailures.OnLevelLoaded((LoadMode)mode);
        }

    }

    public class Mod : IUserMod
    {

        public string Name
        {
            get { ModToolsBootstrap.Bootstrap(); return "ModTools"; }
        }

        public string Description
        {
            get { return "Debugging toolkit for modders"; }
        }

    }

}
