using System.Reflection;
using ColossalFramework;
using ModTools.Utils;
using UnityEngine;

namespace ModTools
{
    internal static class Util
    {
        private static readonly FieldInfo MouseWheeZoomField = ReflectionUtil.FindField(typeof(CameraController), "m_mouseWheelZoom");

        public static void SetMouseScrolling(bool isEnabled)
        {
            if (ToolsModifierControl.cameraController == null)
            {
                return;
            }

            var mouseWheelZoom = (SavedBool)MouseWheeZoomField?.GetValue(ToolsModifierControl.cameraController);
            if (mouseWheelZoom != null && mouseWheelZoom.value != isEnabled)
            {
                Log.Message($"Set mouse scrolling to state {mouseWheelZoom}");
                mouseWheelZoom.value = isEnabled;
            }
        }

        public static bool ComponentIsEnabled(Component component)
        {
            var prop = component.GetType().GetProperty("enabled");
            return prop == null || (bool)prop.GetValue(component, null);
        }
    }
}