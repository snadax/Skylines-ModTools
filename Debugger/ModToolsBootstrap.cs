using System;
using ColossalFramework;
using ColossalFramework.Plugins;
using UnityEngine;

#if !DEBUG
using System.Linq;
using ICities;
#endif

namespace ModTools
{
    public static class ModToolsBootstrap
    {
        public static bool initialized;
        private static bool bootstrapped;

        public static bool IsModToolsActive()
        {
#if DEBUG
            return true;
#else
            return (from item in PluginManager.instance.GetPluginsInfo()
                    let instances = item.GetInstances<IUserMod>()
                    where instances.FirstOrDefault() is Mod
                    select item.isEnabled).FirstOrDefault();
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
            var modToolsGameObject = GameObject.Find("ModTools");
            if (modToolsGameObject != null)
            {
                return;
            }

            modToolsGameObject = new GameObject("ModTools");
            ModTools modTools = modToolsGameObject.AddComponent<ModTools>();
            modTools.Initialize();
        }
    }
}