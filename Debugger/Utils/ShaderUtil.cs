using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ModTools.Utils
{
    internal static class ShaderUtil
    {
        private static readonly Dictionary<string, MethodInfo> Methods = new Dictionary<string, MethodInfo>();
        private static readonly Dictionary<string, FieldInfo> Fields = new Dictionary<string, FieldInfo>();
        private static string[] shaders;

        static ShaderUtil()
        {
            var shaderUtilType = TypeUtil.FindTypeByFullName("ColossalFramework.Packaging.ShaderUtil");
            if (shaderUtilType == null)
            {
                UnityEngine.Debug.LogError("ModTools failed to find type ColossalFramework.Packaging.ShaderUtil!");
                return;
            }

            foreach (var field in shaderUtilType.GetFields(BindingFlags.Public | BindingFlags.NonPublic |
                                                           BindingFlags.Static))
            {
                Fields[field.Name] = field;
            }

            foreach (var method in shaderUtilType.GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                Methods[method.Name] = method;
            }
        }

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

        public static IEnumerable<string> GetTextureProperties()
        {
            return (string[])Fields["textureProps"].GetValue(null);
        }

        public static IEnumerable<string> GetColorProperties()
        {
            return (string[])Fields["colorProps"].GetValue(null);
        }

        public static IEnumerable<string> GetVectorProperties()
        {
            return (string[])Fields["vectorProps"].GetValue(null);
        }

        public static IEnumerable<string> GetFloatProperties()
        {
            return (string[])Fields["floatProps"].GetValue(null);
        }

        public static int CountBoundProperties(this Material material)
        {
            return Call<int>("CountBoundProperties", material);
        }

        private static T Call<T>(string name, params object[] parameters)
        {
            return (T)Methods[name].Invoke(null, parameters);
        }
    }
}