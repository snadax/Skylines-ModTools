using System;
using System.Linq;
using UnityEngine;

namespace ModTools
{
    internal class GUIControls
    {
        private static Configuration config
        {
            get { return ModTools.Instance.config; }
        }

        public static float numberFieldSize = 100;
        public static float stringFieldSize = 200;
        public static float byteFieldSize = 40;
        public static float charFieldSize = 25;

        public delegate void WatchButtonCallback();

        public static string BufferedTextField(string id, string value, float fieldSize)
        {
            GUI.SetNextControlName(id);
            string newValue = GUILayout.TextField(value, GUILayout.Width(fieldSize));
            return value != newValue ? newValue : null;
        }

        public static object EditorValueField(ReferenceChain refChain, string id, Type type, object value)
        {
            if (type == typeof(System.Single))
            {
                var f = (float)value;
                FloatField(id, "", ref f, 0.0f, true, true);
                if (f != (float)value)
                {
                    return f;
                }

                return value;
            }

            if (type == typeof(System.Double))
            {
                var f = (double)value;
                DoubleField(id, "", ref f, 0.0f, true, true);
                if (f != (double)value)
                {
                    return f;
                }

                return value;
            }

            if (type == typeof(System.Byte))
            {
                var f = (byte)value;
                ByteField(id, "", ref f, 0.0f, true, true);
                if (f != (byte)value)
                {
                    return f;
                }

                return value;
            }

            if (type == typeof(System.Int32))
            {
                var f = (int)value;
                IntField(id, "", ref f, 0.0f, true, true);
                if (f != (int)value)
                {
                    return f;
                }

                return value;
            }

            if (type == typeof(System.UInt32))
            {
                var f = (uint)value;
                UIntField(id, "", ref f, 0.0f, true, true);
                if (f != (uint)value)
                {
                    return f;
                }

                return value;
            }

            if (type == typeof(System.Int64))
            {
                var f = (Int64)value;
                Int64Field(id, "", ref f, 0.0f, true, true);
                if (f != (Int64)value)
                {
                    return f;
                }

                return value;
            }

            if (type == typeof(System.UInt64))
            {
                var f = (UInt64)value;
                UInt64Field(id, "", ref f, 0.0f, true, true);
                if (f != (UInt64)value)
                {
                    return f;
                }

                return value;
            }

            if (type == typeof(System.Int16))
            {
                var f = (Int16)value;
                Int16Field(id, "", ref f, 0.0f, true, true);
                if (f != (Int16)value)
                {
                    return f;
                }

                return value;
            }

            if (type == typeof(System.UInt16))
            {
                var f = (UInt16)value;
                UInt16Field(id, "", ref f, 0.0f, true, true);
                if (f != (UInt16)value)
                {
                    return f;
                }

                return value;
            }

            if (type == typeof(System.Boolean))
            {
                var f = (bool)value;
                BoolField("", ref f, 0.0f, true, true);
                if (f != (bool)value)
                {
                    return f;
                }

                return value;
            }

            if (type == typeof(System.String))
            {
                var f = (string)value;
                StringField(id, "", ref f, 0.0f, true, true);
                if (f != (string)value)
                {
                    return f;
                }

                return value;
            }

            if (type == typeof(System.Char))
            {
                var f = (char)value;
                CharField(id, "", ref f, 0.0f, true, true);
                if (f != (char)value)
                {
                    return f;
                }

                return value;
            }

            if (type == typeof(UnityEngine.Vector2))
            {
                var f = (Vector2)value;
                Vector2Field(id, "", ref f, 0.0f, null, true, true);
                if (f != (Vector2)value)
                {
                    return f;
                }

                return value;
            }

            if (type == typeof(UnityEngine.Vector3))
            {
                var f = (Vector3)value;
                Vector3Field(id, "", ref f, 0.0f, null, true, true);
                if (f != (Vector3)value)
                {
                    return f;
                }

                return value;
            }

            if (type == typeof(UnityEngine.Vector4))
            {
                var f = (Vector4)value;
                Vector4Field(id, "", ref f, 0.0f, null, true, true);
                if (f != (Vector4)value)
                {
                    return f;
                }

                return value;
            }

            if (type == typeof(UnityEngine.Quaternion))
            {
                var f = (Quaternion)value;
                QuaternionField(id, "", ref f, 0.0f, null, true, true);
                if (f != (Quaternion)value)
                {
                    return f;
                }

                return value;
            }

            if (type == typeof(UnityEngine.Color))
            {
                var f = (Color)value;
                ColorField(id, "", ref f, 0.0f, null, true, true, color => { refChain.SetValue(color); });
                if (f != (Color)value)
                {
                    return f;
                }

                return value;
            }

            if (type == typeof(UnityEngine.Color32))
            {
                var f = (Color32)value;
                Color32Field(id, "", ref f, 0.0f, null, true, true, color => { refChain.SetValue(color); });
                var v = (Color32)value;
                if (f.r != v.r || f.g != v.g || f.b != v.b || f.a != v.a)
                {
                    return f;
                }

                return value;
            }

            if (type.IsEnum)
            {
                var f = value;
                EnumField(id, "", ref f, 0.0f, true, true);
                if (f != value)
                {
                    return f;
                }

                return value;
            }

            return value;
        }

        public static void FloatField(string id, string name, ref float value, float ident = 0.0f, bool noSpace = false, bool noTypeLabel = false)
        {
            GUILayout.BeginHorizontal();

            if (ident != 0.0f)
            {
                GUILayout.Space(ident);
            }

            if (!noTypeLabel)
            {
                GUI.contentColor = config.typeColor;
                GUILayout.Label("float");
            }

            GUI.contentColor = config.nameColor;
            GUILayout.Label(name);

            if (!noSpace)
            {
                GUILayout.FlexibleSpace();
            }

            GUI.contentColor = config.valueColor;

            string result = BufferedTextField(id, value.ToString(), numberFieldSize);
            if (result != null)
            {
                if (Single.TryParse(result, out float newValue))
                {
                    value = newValue;
                }
            }

            GUI.contentColor = Color.white;

            GUILayout.EndHorizontal();
        }

        public static void DoubleField(string id, string name, ref double value, float ident = 0.0f, bool noSpace = false, bool noTypeLabel = false)
        {
            GUILayout.BeginHorizontal();

            if (ident != 0.0f)
            {
                GUILayout.Space(ident);
            }

            if (!noTypeLabel)
            {
                GUI.contentColor = config.typeColor;
                GUILayout.Label("double");
            }

            GUI.contentColor = config.nameColor;
            GUILayout.Label(name);

            if (!noSpace)
            {
                GUILayout.FlexibleSpace();
            }

            GUI.contentColor = config.valueColor;

            string result = BufferedTextField(id, value.ToString(), numberFieldSize);
            if (result != null)
            {
                if (Double.TryParse(result, out double newValue))
                {
                    value = newValue;
                }
            }

            GUI.contentColor = Color.white;

            GUILayout.EndHorizontal();
        }

        public static void ByteField(string id, string name, ref byte value, float ident = 0.0f, bool noSpace = false, bool noTypeLabel = false)
        {
            GUILayout.BeginHorizontal();

            if (ident != 0.0f)
            {
                GUILayout.Space(ident);
            }

            if (!noTypeLabel)
            {
                GUI.contentColor = config.typeColor;
                GUILayout.Label("int");
            }

            GUI.contentColor = config.nameColor;

            GUILayout.Label(name);

            if (!noSpace)
            {
                GUILayout.FlexibleSpace();
            }

            byte oldValue = value;

            GUI.contentColor = config.valueColor;
            string result = BufferedTextField(id, value.ToString(), byteFieldSize);
            if (result != null)
            {
                if (Byte.TryParse(result, out byte newValue))
                {
                    value = newValue;
                }
            }

            GUI.contentColor = Color.white;

            GUILayout.EndHorizontal();
        }

        public static void IntField(string id, string name, ref int value, float ident = 0.0f, bool noSpace = false, bool noTypeLabel = false)
        {
            GUILayout.BeginHorizontal();

            if (ident != 0.0f)
            {
                GUILayout.Space(ident);
            }

            if (!noTypeLabel)
            {
                GUI.contentColor = config.typeColor;
                GUILayout.Label("int");
            }

            GUI.contentColor = config.nameColor;

            GUILayout.Label(name);

            if (!noSpace)
            {
                GUILayout.FlexibleSpace();
            }

            GUI.contentColor = config.valueColor;

            string result = BufferedTextField(id, value.ToString(), numberFieldSize);
            if (result != null)
            {
                if (Int32.TryParse(result, out int newValue))
                {
                    value = newValue;
                }
            }

            GUI.contentColor = Color.white;
            GUILayout.EndHorizontal();
        }

        public static void UIntField(string id, string name, ref uint value, float ident = 0.0f, bool noSpace = false, bool noTypeLabel = false)
        {
            GUILayout.BeginHorizontal();

            if (ident != 0.0f)
            {
                GUILayout.Space(ident);
            }

            if (!noTypeLabel)
            {
                GUI.contentColor = config.typeColor;
                GUILayout.Label("uint");
            }

            GUI.contentColor = config.nameColor;

            GUILayout.Label(name);

            if (!noSpace)
            {
                GUILayout.FlexibleSpace();
            }

            GUI.contentColor = config.valueColor;

            string result = BufferedTextField(id, value.ToString(), numberFieldSize);
            if (result != null)
            {
                if (UInt32.TryParse(result, out uint newValue))
                {
                    value = newValue;
                }
            }

            GUI.contentColor = Color.white;

            GUILayout.EndHorizontal();
        }

        public static void Int64Field(string id, string name, ref Int64 value, float ident = 0.0f, bool noSpace = false, bool noTypeLabel = false)
        {
            GUILayout.BeginHorizontal();

            if (ident != 0.0f)
            {
                GUILayout.Space(ident);
            }

            if (!noTypeLabel)
            {
                GUI.contentColor = config.typeColor;
                GUILayout.Label("Int64");
            }

            GUI.contentColor = config.nameColor;

            GUILayout.Label(name);

            if (!noSpace)
            {
                GUILayout.FlexibleSpace();
            }

            GUI.contentColor = config.valueColor;

            string result = BufferedTextField(id, value.ToString(), numberFieldSize);
            if (result != null)
            {
                if (Int64.TryParse(result, out long newValue))
                {
                    value = newValue;
                }
            }

            GUI.contentColor = Color.white;

            GUILayout.EndHorizontal();
        }

        public static void UInt64Field(string id, string name, ref UInt64 value, float ident = 0.0f, bool noSpace = false, bool noTypeLabel = false)
        {
            GUILayout.BeginHorizontal();

            if (ident != 0.0f)
            {
                GUILayout.Space(ident);
            }

            if (!noTypeLabel)
            {
                GUI.contentColor = config.typeColor;
                GUILayout.Label("UInt64");
            }

            GUI.contentColor = config.nameColor;

            GUILayout.Label(name);

            if (!noSpace)
            {
                GUILayout.FlexibleSpace();
            }

            GUI.contentColor = config.valueColor;

            string result = BufferedTextField(id, value.ToString(), numberFieldSize);
            if (result != null)
            {
                if (UInt64.TryParse(result, out ulong newValue))
                {
                    value = newValue;
                }
            }

            GUI.contentColor = Color.white;

            GUILayout.EndHorizontal();
        }

        public static void Int16Field(string id, string name, ref Int16 value, float ident = 0.0f, bool noSpace = false, bool noTypeLabel = false)
        {
            GUILayout.BeginHorizontal();

            if (ident != 0.0f)
            {
                GUILayout.Space(ident);
            }

            if (!noTypeLabel)
            {
                GUI.contentColor = config.typeColor;
                GUILayout.Label("Int16");
            }

            GUI.contentColor = config.nameColor;

            GUILayout.Label(name);

            if (!noSpace)
            {
                GUILayout.FlexibleSpace();
            }

            Int16 oldValue = value;

            GUI.contentColor = config.valueColor;

            string result = BufferedTextField(id, value.ToString(), numberFieldSize);
            if (result != null)
            {
                if (Int16.TryParse(result, out short newValue))
                {
                    value = newValue;
                }
            }

            GUI.contentColor = Color.white;

            GUILayout.EndHorizontal();
        }

        public static void UInt16Field(string id, string name, ref UInt16 value, float ident = 0.0f, bool noSpace = false, bool noTypeLabel = false)
        {
            GUILayout.BeginHorizontal();

            if (ident != 0.0f)
            {
                GUILayout.Space(ident);
            }

            if (!noTypeLabel)
            {
                GUI.contentColor = config.typeColor;
                GUILayout.Label("UInt16");
            }

            GUI.contentColor = config.nameColor;

            GUILayout.Label(name);

            if (!noSpace)
            {
                GUILayout.FlexibleSpace();
            }

            GUI.contentColor = config.valueColor;

            string result = BufferedTextField(id, value.ToString(), numberFieldSize);
            if (result != null)
            {
                if (UInt16.TryParse(result, out ushort newValue))
                {
                    value = newValue;
                }
            }

            GUI.contentColor = Color.white;

            GUILayout.EndHorizontal();
        }

        public static void StringField(string id, string name, ref string value, float ident = 0.0f, bool noSpace = false, bool noTypeLabel = false)
        {
            GUILayout.BeginHorizontal();

            if (ident != 0.0f)
            {
                GUILayout.Space(ident);
            }

            if (!noTypeLabel)
            {
                GUI.contentColor = config.typeColor;
                GUILayout.Label("string");
            }

            GUI.contentColor = config.nameColor;
            GUILayout.Label(name);
            if (!noSpace)
            {
                GUILayout.FlexibleSpace();
            }

            GUI.contentColor = config.valueColor;

            string result = BufferedTextField(id, value.ToString(), stringFieldSize);
            if (result != null)
            {
                value = result;
            }

            GUI.contentColor = Color.white;

            GUILayout.EndHorizontal();
        }

        public static void CharField(string id, string name, ref char value, float ident = 0.0f, bool noSpace = false, bool noTypeLabel = false)
        {
            GUILayout.BeginHorizontal();

            if (ident != 0.0f)
            {
                GUILayout.Space(ident);
            }

            if (!noTypeLabel)
            {
                GUI.contentColor = config.typeColor;
                GUILayout.Label("string");
            }

            GUI.contentColor = config.nameColor;
            GUILayout.Label(name);
            if (!noSpace)
            {
                GUILayout.FlexibleSpace();
            }

            GUI.contentColor = config.valueColor;

            string result = BufferedTextField(id, value.ToString(), charFieldSize);
            if (result != null)
            {
                value = result[0];
            }

            GUI.contentColor = Color.white;

            GUILayout.EndHorizontal();
        }

        public static void BoolField(string name, ref bool value, float ident = 0.0f, bool noSpace = false, bool noTypeLabel = false)
        {
            GUILayout.BeginHorizontal();

            if (ident != 0.0f)
            {
                GUILayout.Space(ident);
            }

            if (!noTypeLabel)
            {
                GUI.contentColor = config.typeColor;
                GUILayout.Label("bool");
            }

            GUI.contentColor = config.nameColor;
            GUILayout.Label(name);

            GUI.contentColor = config.valueColor;

            if (!noSpace)
            {
                GUILayout.FlexibleSpace();
            }

            value = GUILayout.Toggle(value, "");

            GUI.contentColor = Color.white;

            GUILayout.EndHorizontal();
        }

        public static void EnumField(string id, string name, ref object value, float ident = 0.0f, bool noSpace = false, bool noTypeLabel = false)
        {
            GUILayout.BeginHorizontal();

            try
            {
                if (ident != 0.0f)
                {
                    GUILayout.Space(ident);
                }

                var enumType = value.GetType();

                if (!noTypeLabel)
                {
                    GUI.contentColor = config.typeColor;
                    GUILayout.Label(enumType.FullName);
                }

                GUI.contentColor = config.nameColor;
                GUILayout.Label(name);
                GUI.contentColor = config.valueColor;

                if (!noSpace)
                {
                    GUILayout.FlexibleSpace();
                }

                var enumNames = Enum.GetNames(enumType).ToArray();

                if (TypeUtil.IsBitmaskEnum(enumType))
                {
                    GUILayout.Label(value.ToString());
                }
                else
                {
                    int i = 0;
                    for (; i < enumNames.Length; i++)
                    {
                        if (value.ToString() == enumNames[i])
                        {
                            break;
                        }
                    }

                    int newIndex = GUIComboBox.Box(i, enumNames, id);
                    value = Enum.Parse(enumType, enumNames[newIndex]);
                }

                GUI.contentColor = Color.white;
            }
            catch (Exception)
            {
                GUILayout.EndHorizontal();
                throw;
            }

            GUILayout.EndHorizontal();
        }

        public static void Vector2Field(string id, string name, ref Vector2 value, float ident = 0.0f, WatchButtonCallback watch = null, bool noSpace = false, bool noTypeLabel = false)
        {
            GUILayout.BeginHorizontal();

            if (ident != 0.0f)
            {
                GUILayout.Space(ident);
            }

            if (!noTypeLabel)
            {
                GUI.contentColor = config.typeColor;
                GUILayout.Label("Vector2");
            }

            GUI.contentColor = config.nameColor;
            GUILayout.Label(name);

            GUI.contentColor = config.valueColor;

            if (!noSpace)
            {
                GUILayout.FlexibleSpace();
            }

            FloatField(id + ".x", "x", ref value.x, 0.0f, true, true);
            FloatField(id + ".y", "y", ref value.y, 0.0f, true, true);

            if (watch != null)
            {
                if (GUILayout.Button("Watch"))
                {
                    watch();
                }
            }

            GUI.contentColor = Color.white;

            GUILayout.EndHorizontal();
        }

        public static void Vector3Field(string id, string name, ref Vector3 value, float ident = 0.0f, WatchButtonCallback watch = null, bool noSpace = false, bool noTypeLabel = false)
        {
            GUILayout.BeginHorizontal();

            if (ident != 0.0f)
            {
                GUILayout.Space(ident);
            }

            if (!noTypeLabel)
            {
                GUI.contentColor = config.typeColor;
                GUILayout.Label("Vector3");
            }

            GUI.contentColor = config.nameColor;
            GUILayout.Label(name);

            GUI.contentColor = config.valueColor;

            if (!noSpace)
            {
                GUILayout.FlexibleSpace();
            }

            FloatField(id + ".x", "x", ref value.x, 0.0f, true, true);
            FloatField(id + ".y", "y", ref value.y, 0.0f, true, true);
            FloatField(id + ".z", "z", ref value.z, 0.0f, true, true);

            if (watch != null)
            {
                if (GUILayout.Button("Watch"))
                {
                    watch();
                }
            }

            GUI.contentColor = Color.white;

            GUILayout.EndHorizontal();
        }

        public static void Vector4Field(string id, string name, ref Vector4 value, float ident = 0.0f, WatchButtonCallback watch = null, bool noSpace = false, bool noTypeLabel = false)
        {
            GUILayout.BeginHorizontal();

            if (ident != 0.0f)
            {
                GUILayout.Space(ident);
            }

            if (!noTypeLabel)
            {
                GUI.contentColor = config.typeColor;
                GUILayout.Label("Vector4");
            }

            GUI.contentColor = config.nameColor;
            GUILayout.Label(name);

            GUI.contentColor = config.valueColor;

            if (!noSpace)
            {
                GUILayout.FlexibleSpace();
            }

            FloatField(id + ".x", "x", ref value.x, 0.0f, true, true);
            FloatField(id + ".y", "y", ref value.y, 0.0f, true, true);
            FloatField(id + ".z", "z", ref value.z, 0.0f, true, true);
            FloatField(id + ".w", "w", ref value.w, 0.0f, true, true);

            if (watch != null)
            {
                if (GUILayout.Button("Watch"))
                {
                    watch();
                }
            }

            GUI.contentColor = Color.white;

            GUILayout.EndHorizontal();
        }

        public static void QuaternionField(string id, string name, ref Quaternion value, float ident = 0.0f, WatchButtonCallback watch = null, bool noSpace = false, bool noTypeLabel = false)
        {
            GUILayout.BeginHorizontal();

            if (ident != 0.0f)
            {
                GUILayout.Space(ident);
            }

            if (!noTypeLabel)
            {
                GUI.contentColor = config.typeColor;
                GUILayout.Label("Quaternion (euler)");
            }

            GUI.contentColor = config.nameColor;
            GUILayout.Label(name);

            GUI.contentColor = config.valueColor;

            if (!noSpace)
            {
                GUILayout.FlexibleSpace();
            }

            var euler = value.eulerAngles;

            FloatField(id + ".x", "x", ref euler.x, 0.0f, true, true);
            FloatField(id + ".y", "y", ref euler.y, 0.0f, true, true);
            FloatField(id + ".z", "z", ref euler.z, 0.0f, true, true);

            if (euler != value.eulerAngles)
            {
                value = Quaternion.Euler(euler);
            }

            if (watch != null)
            {
                if (GUILayout.Button("Watch"))
                {
                    watch();
                }
            }

            GUI.contentColor = Color.white;

            GUILayout.EndHorizontal();
        }

        public static void ColorField(string id, string name, ref Color value, float ident = 0.0f, WatchButtonCallback watch = null, bool noSpace = false, bool noTypeLabel = false, ColorPicker.OnColorChanged onColorChanged = null)
        {
            GUILayout.BeginHorizontal();

            if (ident != 0.0f)
            {
                GUILayout.Space(ident);
            }

            if (!noTypeLabel)
            {
                GUI.contentColor = config.typeColor;
                GUILayout.Label("Color");
            }

            GUI.contentColor = config.nameColor;
            GUILayout.Label(name);

            GUI.contentColor = config.valueColor;

            if (!noSpace)
            {
                GUILayout.FlexibleSpace();
            }

            var r = (byte)(Mathf.Clamp(value.r * 255.0f, Byte.MinValue, Byte.MaxValue));
            var g = (byte)(Mathf.Clamp(value.g * 255.0f, Byte.MinValue, Byte.MaxValue));
            var b = (byte)(Mathf.Clamp(value.b * 255.0f, Byte.MinValue, Byte.MaxValue));
            var a = (byte)(Mathf.Clamp(value.a * 255.0f, Byte.MinValue, Byte.MaxValue));

            ByteField(id + ".r", "r", ref r, 0.0f, true, true);
            ByteField(id + ".g", "g", ref g, 0.0f, true, true);
            ByteField(id + ".b", "b", ref b, 0.0f, true, true);
            ByteField(id + ".a", "a", ref a, 0.0f, true, true);

            value.r = Mathf.Clamp01(r / 255.0f);
            value.g = Mathf.Clamp01(g / 255.0f);
            value.b = Mathf.Clamp01(b / 255.0f);
            value.a = Mathf.Clamp01(a / 255.0f);

            if (onColorChanged != null)
            {
                if (value.r != r || value.g != g || value.b != b || value.a != a)
                {
                    onColorChanged(value);
                }

                if (GUILayout.Button("", GUILayout.Width(72)))
                {
                    var picker = ModTools.Instance.colorPicker;
                    if (picker != null)
                    {
                        picker.SetColor(value, onColorChanged);

                        Vector2 mouse = Input.mousePosition;
                        mouse.y = Screen.height - mouse.y;

                        picker.rect.position = mouse;
                        picker.visible = true;
                    }

                }

                var lastRect = GUILayoutUtility.GetLastRect();
                lastRect.x += 4.0f;
                lastRect.y += 4.0f;
                lastRect.width -= 8.0f;
                lastRect.height -= 8.0f;
                GUI.DrawTexture(lastRect, ColorPicker.GetColorTexture(id, value), ScaleMode.StretchToFill);
            }

            if (watch != null)
            {
                if (GUILayout.Button("Watch"))
                {
                    watch();
                }
            }

            GUI.contentColor = Color.white;

            GUILayout.EndHorizontal();
        }

        public static void Color32Field(string id, string name, ref Color32 value, float ident = 0.0f, WatchButtonCallback watch = null, bool noSpace = false, bool noTypeLabel = false, ColorPicker.OnColor32Changed onColor32Changed = null)
        {
            GUILayout.BeginHorizontal();

            if (ident != 0.0f)
            {
                GUILayout.Space(ident);
            }

            if (!noTypeLabel)
            {
                GUI.contentColor = config.typeColor;
                GUILayout.Label("Color");
            }

            GUI.contentColor = config.nameColor;
            GUILayout.Label(name);

            GUI.contentColor = config.valueColor;

            if (!noSpace)
            {
                GUILayout.FlexibleSpace();
            }

            ByteField(id + ".r", "r", ref value.r, 0.0f, true, true);
            ByteField(id + ".g", "g", ref value.g, 0.0f, true, true);
            ByteField(id + ".b", "b", ref value.b, 0.0f, true, true);
            ByteField(id + ".a", "a", ref value.a, 0.0f, true, true);

            if (onColor32Changed != null)
            {
                if (GUILayout.Button("", GUILayout.Width(72)))
                {
                    var picker = ModTools.Instance.colorPicker;
                    if (picker != null)
                    {
                        picker.SetColor(value, onColor32Changed);

                        Vector2 mouse = Input.mousePosition;
                        mouse.y = Screen.height - mouse.y;

                        picker.rect.position = mouse;
                        picker.visible = true;
                    }

                }

                var lastRect = GUILayoutUtility.GetLastRect();
                lastRect.x += 4.0f;
                lastRect.y += 4.0f;
                lastRect.width -= 8.0f;
                lastRect.height -= 8.0f;
                GUI.DrawTexture(lastRect, ColorPicker.GetColorTexture(id, value), ScaleMode.StretchToFill);
            }

            if (watch != null)
            {
                if (GUILayout.Button("Watch"))
                {
                    watch();
                }
            }

            GUI.contentColor = Color.white;

            GUILayout.EndHorizontal();
        }
    }

}