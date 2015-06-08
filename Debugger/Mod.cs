using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace ModTools
{

    public class ModToolsBootstrap
    {

        private static GameObject modToolsGameObject;
        private static ModTools modTools;

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

            try
            {
                CODebugBase<LogChannel>.verbose = true;
                CODebugBase<LogChannel>.EnableChannels(LogChannel.All);

                InitModTools(SimulationManager.UpdateMode.Undefined);

                var target = typeof(LoadingWrapper).GetMethod("OnLevelLoaded",
                new[] { typeof(SimulationManager.UpdateMode) });

                var replacement = typeof(ModToolsBootstrap).GetMethod("OnLevelLoaded",
                    new[] { typeof(SimulationManager.UpdateMode) });

                RedirectionHelper.RedirectCalls(target, replacement);
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

            var loadingManager = LoadingManager.instance;
            var wrapper = loadingManager.m_LoadingWrapper;

            var loadingExtensions = Util.GetPrivate<List<ILoadingExtension>>(wrapper, "m_LoadingExtensions");

            try
            {
                for (int i = 0; i < loadingExtensions.Count; i++)
                {
                    loadingExtensions[i].OnLevelLoaded((LoadMode)mode);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                UIView.ForwardException(new ModException("A Mod caused an error", ex));
            }
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
