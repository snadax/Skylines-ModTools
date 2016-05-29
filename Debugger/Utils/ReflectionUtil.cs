using System.Reflection;

namespace ModTools.Utils
{
    public static class ReflectionUtil
    {
        public static FieldInfo FindField<T>(T o, string fieldName)
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var f in fields)
            {
                if (f.Name == fieldName)
                {
                    return f;
                }
            }

            return null;
        }

        public static T GetFieldValue<T>(FieldInfo field, object o)
        {
            return (T)field.GetValue(o);
        }

        public static void SetFieldValue(FieldInfo field, object o, object value)
        {
            field.SetValue(o, value);
        }

        public static Q GetPrivate<Q>(object o, string fieldName)
        {
            var fields = o.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field = null;

            foreach (var f in fields)
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
            var fields = o.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field = null;

            foreach (var f in fields)
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