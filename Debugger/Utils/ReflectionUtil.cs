using System;
using System.Reflection;

namespace ModTools.Utils
{
    public static class ReflectionUtil
    {
        public static FieldInfo FindField<T>(string fieldName)
            => Array.Find(typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance), f => f.Name == fieldName);

        public static T GetFieldValue<T>(FieldInfo field, object o) => (T)field.GetValue(o);

        public static void SetFieldValue(FieldInfo field, object o, object value) => field.SetValue(o, value);

        public static Q GetPrivate<Q>(object o, string fieldName)
        {
            FieldInfo[] fields = o.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field = null;

            foreach (FieldInfo f in fields)
            {
                if (f.Name == fieldName)
                {
                    field = f;
                    break;
                }
            }

            return (Q)field.GetValue(o);
        }

        public static void SetPrivate(object o, string fieldName, object value)
        {
            FieldInfo[] fields = o.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field = null;

            foreach (FieldInfo f in fields)
            {
                if (f.Name == fieldName)
                {
                    field = f;
                    break;
                }
            }

            field.SetValue(o, value);
        }
    }
}