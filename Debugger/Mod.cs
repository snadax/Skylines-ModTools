using System;
using ColossalFramework;
using ColossalFramework.Plugins;
using ICities;
using UnityEngine;

namespace ModTools
{
    public sealed class Mod : IUserMod
    {
        private const string ModToolsName = "ModTools";

        private GameObject mainObject;

        public string Name => ModToolsName;

        public string Description => "Debugging toolkit for modders";

        public void OnEnabled()
        {
            try
            {
                if (mainObject != null)
                {
                    return;
                }

                CODebugBase<LogChannel>.verbose = true;
                CODebugBase<LogChannel>.EnableChannels(LogChannel.All);

                mainObject = new GameObject(ModToolsName);
                UnityEngine.Object.DontDestroyOnLoad(mainObject);

                var modTools = mainObject.AddComponent<ModTools>();
                modTools.Initialize();
            }
            catch (Exception e)
            {
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Error, e.Message);
            }
        }

        public void OnDisabled()
        {
            if (mainObject != null)
            {
                CODebugBase<LogChannel>.verbose = false;
                UnityEngine.Object.Destroy(mainObject);
                mainObject = null;
            }
        }
    }
}