using UnityEngine;

namespace ModTools
{
    internal static class Util
    {
        public static bool ComponentIsEnabled(Component component)
        {
            var prop = component.GetType().GetProperty("enabled");
            return prop == null || (bool)prop.GetValue(component, null);
        }
    }
}