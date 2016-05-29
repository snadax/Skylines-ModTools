using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.IO;
using ColossalFramework.Plugins;
using ICities;
using ModTools.Utils;
using UnityEngine;
using UnityExtension;

namespace ModTools
{

    public static class Util
    {


        public static void SetMouseScrolling(bool isEnabled)
        {
            try
            {
                var mouseWheelZoom = ReflectionUtil.GetPrivate<SavedBool>(ToolsModifierControl.cameraController, "m_mouseWheelZoom");
                if (mouseWheelZoom.value != isEnabled)
                {
                    mouseWheelZoom.value = isEnabled;
                }
            }
            catch (Exception)
            {
            }
        }

        public static bool ComponentIsEnabled(Component component)
        {
            var prop = component.GetType().GetProperty("enabled");
            if (prop == null)
            {
                return true;
            }

            return (bool)prop.GetValue(component, null);
        }

        public static string ModToolsAssemblyPath
        {
            get
            {
                var pluginManager = PluginManager.instance;
                var plugins = pluginManager.GetPluginsInfo();

                foreach (var item in plugins)
                {
                    var instances = item.GetInstances<IUserMod>();
                    if (!(instances.FirstOrDefault() is Mod))
                    {
                        continue;
                    }
                    foreach (var file in Directory.GetFiles(item.modPath))
                    {
                        if (Path.GetExtension(file) == ".dll")
                        {
                            return file;
                        }
                    }
                }
                throw new Exception("Failed to find ModTools assembly!");

            }
        }
    }

}
