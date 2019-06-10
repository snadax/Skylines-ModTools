using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ModTools
{
    public static class TypeUtil
    {
        public static bool IsSpecialType(Type t)
        {
            return t.IsPrimitive
                || t.IsEnum
                || t == typeof(string)
                || t == typeof(UnityEngine.Vector2)
                || t == typeof(UnityEngine.Vector3)
                || t == typeof(UnityEngine.Vector4)
                || t == typeof(UnityEngine.Quaternion)
                || t == typeof(UnityEngine.Color)
                || t == typeof(UnityEngine.Color32);
        }

        public static bool IsBitmaskEnum(Type t) => t.IsDefined(typeof(FlagsAttribute), false);

        public static bool IsTextureType(Type t)
        {
            return t == typeof(UnityEngine.Texture) || t == typeof(UnityEngine.Texture2D)
                   || t == typeof(UnityEngine.RenderTexture) || t == typeof(UnityEngine.Texture3D) || t == typeof(UnityEngine.Cubemap);
        }

        public static bool IsMeshType(Type t) => t == typeof(UnityEngine.Mesh);

        public static MemberInfo[] GetAllMembers(Type type, bool recursive = false)
            => GetMembersInternal(type, recursive, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

        public static MemberInfo[] GetPublicMembers(Type type, bool recursive = false)
            => GetMembersInternal(type, recursive, BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);

        public static MemberInfo[] GetPrivateMembers(Type type, bool recursive = false)
            => GetMembersInternal(type, recursive, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);

        public static void ClearTypeCache() => _typeCache = new Dictionary<Type, MemberInfo[]>();

        private static Dictionary<Type, MemberInfo[]> _typeCache = new Dictionary<Type, MemberInfo[]>();

        private static MemberInfo[] GetMembersInternal(Type type, bool recursive, BindingFlags bindingFlags)
        {
            if (_typeCache.ContainsKey(type))
            {
                return _typeCache[type];
            }

            var results = new Dictionary<string, MemberInfo>();
            GetMembersInternal2(type, recursive, bindingFlags, results);
            MemberInfo[] members = results.Values.ToArray();
            _typeCache[type] = members;
            return members;
        }

        private static void GetMembersInternal2(Type type, bool recursive, BindingFlags bindingFlags, Dictionary<string, MemberInfo> outResults)
        {
            foreach (MemberInfo member in type.GetMembers(bindingFlags))
            {
                if (!outResults.ContainsKey(member.Name))
                {
                    outResults.Add(member.Name, member);
                }
            }

            if (recursive && type.BaseType != null)
            {
                GetMembersInternal2(type.BaseType, true, bindingFlags, outResults);
            }
        }

        public static bool IsEnumerable(object myProperty)
        {
            return typeof(IEnumerable).IsInstanceOfType(myProperty)
                || typeof(IEnumerable<>).IsInstanceOfType(myProperty);
        }

        public static bool IsCollection(object myProperty)
        {
            return typeof(ICollection).IsAssignableFrom(myProperty.GetType())
                || typeof(ICollection<>).IsAssignableFrom(myProperty.GetType());
        }

        public static bool IsList(object myProperty)
        {
            return typeof(IList).IsAssignableFrom(myProperty.GetType())
                || typeof(IList<>).IsAssignableFrom(myProperty.GetType());
        }
    }
}