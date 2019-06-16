using System;
using System.Reflection;

namespace ModTools.Utils
{
    internal static class ReflectionUtil
    {
        public static FieldInfo FindField(Type type, string fieldName)
            => Array.Find(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance), f => f.Name == fieldName);
    }
}