using System;
using System.Linq;
using ColossalFramework;
using ColossalFramework.Plugins;
using ICities;
using UnityEngine;

namespace ModTools
{
    public static class ModToolsBootstrap
    {

        private static GameObject modToolsGameObject;
        private static ModTools modTools;
        public static bool initialized;
        private static bool bootstrapped;

        public static bool IsModToolsActive()
        {
#if DEBUG
            return true;
#else
            var pluginManager = PluginManager.instance;
            var plugins = pluginManager.GetPluginsInfo();

            return (from item in plugins let instances = item.GetInstances<IUserMod>()
                    where instances.FirstOrDefault() is Mod select item.isEnabled).FirstOrDefault();

#endif
        }

        public static void Bootstrap()
        {
            if (!bootstrapped)
            {
                CODebugBase<LogChannel>.verbose = true;
                CODebugBase<LogChannel>.EnableChannels(LogChannel.All);
                bootstrapped = true;
            }
            if (initialized)
            {
                return;
            }
            try
            {
                InitModTools();
                initialized = true;
            }
            catch (Exception e)
            {
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Error, e.Message);
            }
        }

        private static void InitModTools()
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
            modTools.Initialize();
        }

    }
}
