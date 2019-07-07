using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ModTools.Utils
{
    internal static class TypeUtil
    {
        private static Dictionary<Type, MemberInfo[]> typeCache = new Dictionary<Type, MemberInfo[]>();

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

        public static MemberInfo[] GetAllMembers(Type type, bool recursive = false)
            => GetMembersInternal(type, recursive, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

        public static void ClearTypeCache() => typeCache = new Dictionary<Type, MemberInfo[]>();

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

        public static FieldInfo FindField(Type type, string fieldName)
            => Array.Find(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance), f => f.Name == fieldName);

        private static MemberInfo[] GetMembersInternal(Type type, bool recursive, BindingFlags bindingFlags)
        {
            if (typeCache.ContainsKey(type))
            {
                return typeCache[type];
            }

            var results = new Dictionary<string, MemberInfo>();
            GetMembersInternal2(type, recursive, bindingFlags, results);
            var members = results.Values.ToArray();
            typeCache[type] = members;
            return members;
        }

        private static void GetMembersInternal2(Type type, bool recursive, BindingFlags bindingFlags, Dictionary<string, MemberInfo> outResults)
        {
            foreach (var member in type.GetMembers(bindingFlags))
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
    }
}