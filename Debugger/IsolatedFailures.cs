using System;
using System.Collections.Generic;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace ModTools
{
    public class IsolatedFailures
    {
        public static void OnLevelLoaded(LoadMode mode)
        {
            Func<ILoadingExtension, LoadMode, Boolean> callback = (e, m) =>
            {
                e.OnLevelLoaded(m);
                return true;
            };
            ProcessLoadingExtensions(mode, callback);
        }

        private static void ProcessLoadingExtensions(LoadMode mode, Func<ILoadingExtension, LoadMode, bool> callback)
        {
            var loadingExtensions = Util.GetPrivate<List<ILoadingExtension>>(LoadingManager.instance.m_LoadingWrapper,
                "m_LoadingExtensions");
            var exceptions = new List<Exception>();
            foreach (var loadingExtension in loadingExtensions)
            {
                try
                {
                    callback(loadingExtension, mode);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    exceptions.Add(ex);
                }
            }
            if (exceptions.Count <= 0)
            {
                return;
            }
            var mergedTraces = "";
            var i = 0;
            exceptions.ForEach((e) =>
            {
                if (e == null)
                {
                    mergedTraces += String.Format("\n---------------------------\n[{0}]: <null exception>", i);
                }
                else
                {
                    mergedTraces += String.Format("\n---------------------------\n[{0}]: {1}\n{2}", i, e.Message, e.StackTrace);
                }
                ++i;
            });
            UIView.ForwardException(new ModException(String.Format("{0} - Some mods caused errors:", mode),
                new Exception(mergedTraces)));
        }
    }
}