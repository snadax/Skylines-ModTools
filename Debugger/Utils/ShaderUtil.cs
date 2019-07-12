using System.Linq;
using UnityEngine;

namespace ModTools.Utils
{
    internal static class ShaderUtil
    {
        private static string[] shaders = null;
        
        public static void ClearShaderCache()
        {
            shaders = null;
        }
        
        public static string[] GetShaders()
        {
            return shaders ?? (shaders =
                       Resources.FindObjectsOfTypeAll<Shader>()
                           .Select(shader => shader.name)
                           .OrderBy(name => name)
                           .ToArray());
        }
    }
}