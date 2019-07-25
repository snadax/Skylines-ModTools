using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ModTools.Utils
{
    internal static class ShaderUtil
    {
        private static string[] shaders;
        private static readonly Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();
        private static readonly Dictionary<string, FieldInfo> fields = new Dictionary<string, FieldInfo>();

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
                fields[field.Name] = field;
            }

            foreach (var method in shaderUtilType.GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                methods[method.Name] = method;
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
            return (string[])fields["textureProps"].GetValue(null);
        }
        
        public static IEnumerable<string> GetColorProperties()
        {
            return (string[])fields["colorProps"].GetValue(null);
        }
        
        public static IEnumerable<string> GetVectorProperties()
        {
            return (string[])fields["vectorProps"].GetValue(null);
        }
        
        public static IEnumerable<string> GetFloatProperties()
        {
            return (string[])fields["floatProps"].GetValue(null);
        }

        public static int CountBoundProperties(this Material material)
        {
            return Call<int>("CountBoundProperties", material);
        }

        private static T Call<T>(string name, params object[] parameters)
        {
            return (T)methods[name].Invoke(null, parameters);
        }
    }
}