using System;
using ColossalFramework;
using ColossalFramework.Plugins;
using ICities;
using ModTools.Utils;
using UnityEngine;

namespace ModTools
{
    public sealed class ModToolsMod : IUserMod
    {
        public const string ModToolsName = "ModTools";

        public static GameObject MainWindowObject;

        private static GameObject mainObject;

        public static string Version { get; } = GitVersion.GetAssemblyVersion(typeof(ModToolsMod).Assembly);

        public string Name => ModToolsName;

        public string Description => "Debugging toolkit for modders, version " + Version;

        public void OnEnabled()
        {
            try
            {
                if (MainWindowObject != null)
                {
                    return;
                }

                CODebugBase<LogChannel>.verbose = true;
                CODebugBase<LogChannel>.EnableChannels(LogChannel.All);

                mainObject = new GameObject(ModToolsName);
                UnityEngine.Object.DontDestroyOnLoad(mainObject);

                MainWindowObject = new GameObject(ModToolsName + nameof(MainWindow));
                UnityEngine.Object.DontDestroyOnLoad(MainWindowObject);

                var modTools = MainWindowObject.AddComponent<MainWindow>();
                modTools.Initialize();
            }
            catch (Exception e)
            {
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Error, e.Message);
            }
        }

        public void OnDisabled()
        {
            if (MainWindowObject != null)
            {
                CODebugBase<LogChannel>.verbose = false;
                UnityEngine.Object.Destroy(MainWindowObject);
                MainWindowObject = null;
            }

            if (mainObject != null)
            {
                CODebugBase<LogChannel>.verbose = false;
                UnityEngine.Object.Destroy(mainObject);
                mainObject = null;
            }
        }
    }
}