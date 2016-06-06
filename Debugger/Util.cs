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


    }

}
