using ColossalFramework;
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
                SavedBool mouseWheelZoom = ReflectionUtil.GetPrivate<SavedBool>(ToolsModifierControl.cameraController, "m_mouseWheelZoom");
                if (mouseWheelZoom.value != isEnabled)
                {
                    mouseWheelZoom.value = isEnabled;
                }
            }
            catch
            {
            }
        }

        public static bool ComponentIsEnabled(Component component)
        {
            System.Reflection.PropertyInfo prop = component.GetType().GetProperty("enabled");
            return prop == null || (bool)prop.GetValue(component, null);
        }
    }
}