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
        public static bool inMainMenu = true;

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
                    bootstrapped = true;
                }
                if (inMainMenu)
                {
                    InitModTools();
                }
                initialized = true;
            }
            catch (Exception ex)
            {
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Error, ex.Message);
            }
        }

        public static void InitModTools()
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
