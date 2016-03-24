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
    public class LoadingWrapperDetour
    {

        public static void Deploy()
        {
            RedirectionHelper.RedirectCalls(
                typeof(LoadingWrapper).GetMethod("OnLevelLoaded", new[] { typeof(SimulationManager.UpdateMode) }),
                typeof(LoadingWrapperDetour).GetMethod("OnLevelLoaded", new[] { typeof(SimulationManager.UpdateMode) }));
        }

        public void OnLevelLoaded(SimulationManager.UpdateMode mode)
        {
            var m_LoadingExtensions = Util.GetPrivate<List<ILoadingExtension>>(LoadingManager.instance.m_LoadingWrapper, "m_LoadingExtensions");
            var modTools = m_LoadingExtensions.Find((e) => e is Mod);
            if (modTools != null)
            {
                ProcessLoadingExtension(modTools, mode);
            }
            foreach (var extension in m_LoadingExtensions.Where(extension => extension != modTools))
            {
                ProcessLoadingExtension(extension, mode);
            }
        }


        private static void ProcessLoadingExtension(ILoadingExtension extension, SimulationManager.UpdateMode mode)
        {
            try
            {
                extension.OnLevelLoaded((LoadMode)mode);
            }
            catch (Exception ex)
            {
                PluginManager.PluginInfo pluginInfo = Singleton<PluginManager>.instance.FindPluginInfo(extension.GetType().Assembly);
                if (pluginInfo != null)
                {
                    ModException modException = new ModException("The Mod " + pluginInfo.ToString() + " has caused an error", ex);
                    UIView.ForwardException((Exception)modException);
                    Debug.LogException((Exception)modException);
                }
                else
                {
                    Debug.LogException(ex);
                    UIView.ForwardException((Exception)new ModException("A Mod caused an error", ex));
                }
            }
        }
    }
}