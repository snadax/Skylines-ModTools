using System;
using ModTools.Utils;
using UnityEngine;

namespace ModTools
{
    internal static class GUIControls
    {
        private const float NumberFieldSize = 100f;
        private const float StringFieldSize = 200f;
        private const float ByteFieldSize = 40f;
        private const float CharFieldSize = 25f;

        private static string lastFocusedFieldId;
        private static bool lastFocusedFieldEmpty;

        public delegate T ValuePresenterDelegate<T>(string id, T value)
            where T : struct;

        private static ModConfiguration Config => ModTools.Instance.Config;

        public static T PrimitiveValueField<T>(string id, string name, T value)
            where T : struct
        {
            GUILayout.BeginHorizontal();

            if (!string.IsNullOrEmpty(name))
            {
                GUI.contentColor = Config.NameColor;
                GUILayout.Label(name);
            }

            GUI.contentColor = Config.ValueColor;

            var newText = BufferedTextField(id, value.ToString(), GetTextFieldSize(typeof(T)));

            GUI.contentColor = Color.white;

            GUILayout.EndHorizontal();

            return ParseHelper.TryParse<T>(newText, out var newValue) ? newValue : value;
        }

        public static object PrimitiveValueField(string id, string name, object value)
        {
            GUILayout.BeginHorizontal();

            if (!string.IsNullOrEmpty(name))
            {
                GUI.contentColor = Config.NameColor;
                GUILayout.Label(name);
            }

            GUI.contentColor = Config.ValueColor;

            var valueType = value.GetType();
            var newText = BufferedTextField(id, value.ToString(), GetTextFieldSize(valueType));

            GUI.contentColor = Color.white;

            GUILayout.EndHorizontal();

            return ParseHelper.TryParse(newText, valueType, out var newValue) ? newValue : value;
        }

        public static object EditorValueField(string id, Type type, object value)
        {
            if (type.IsPrimitive && type != typeof(bool) || type == typeof(string))
            {
                return PrimitiveValueField(id, string.Empty, value);
            }

            if (type == typeof(bool))
            {
                return BoolField(string.Empty, (bool)value);
            }

            if (type.IsEnum)
            {
                return EnumField(id, string.Empty, value);
            }

            if (type == typeof(Vector2))
            {
                return CustomValueField(id, string.Empty, PresentVector2, (Vector2)value);
            }

            if (type == typeof(Vector3))
            {
                return CustomValueField(id, string.Empty, PresentVector3, (Vector3)value);
            }

            if (type == typeof(Vector4))
            {
                return CustomValueField(id, string.Empty, PresentVector4, (Vector4)value);
            }

            if (type == typeof(Quaternion))
            {
                return CustomValueField(id, string.Empty, PresentQuaternion, (Quaternion)value);
            }

            if (type == typeof(Color))
            {
                return CustomValueField(id, string.Empty, PresentColor, (Color)value);
            }

            if (type == typeof(Color32))
            {
                return (Color32)CustomValueField(id, string.Empty, PresentColor, (Color)(Color32)value);
            }

            return value;
        }

        public static bool BoolField(string name, bool value)
        {
            GUILayout.BeginHorizontal();

            if (!string.IsNullOrEmpty(name))
            {
                GUI.contentColor = Config.NameColor;
                GUILayout.Label(name);
            }

            GUI.contentColor = Config.ValueColor;

            value = GUILayout.Toggle(value, string.Empty);

            GUI.contentColor = Color.white;

            GUILayout.EndHorizontal();

            return value;
        }

        public static object EnumField(string id, string name, object value)
        {
            GUILayout.BeginHorizontal();

            try
            {
                var enumType = value.GetType();

                if (!string.IsNullOrEmpty(name))
                {
                    GUI.contentColor = Config.NameColor;
                    GUILayout.Label(name);
                }

                GUI.contentColor = Config.ValueColor;

                if (TypeUtil.IsBitmaskEnum(enumType))
                {
                    GUILayout.Label(value.ToString());
                }
                else
                {
                    var names = Enum.GetNames(enumType);
                    var values = Enum.GetValues(enumType);
                    var valueIndex = Array.IndexOf(values, value);
                    var newValueIndex = GUIComboBox.Box(valueIndex, names, id);
                    if (newValueIndex != valueIndex)
                    {
                        value = values.GetValue(newValueIndex);
                    }
                }

                GUI.contentColor = Color.white;
            }
            finally
            {
                GUILayout.EndHorizontal();
            }

            return value;
        }

        public static Vector2 PresentVector2(string id, Vector2 value)
        {
            value.x = PrimitiveValueField(id + ".x", "x", value.x);
            value.y = PrimitiveValueField(id + ".y", "y", value.y);
            return value;
        }

        public static Vector3 PresentVector3(string id, Vector3 value)
        {
            value.x = PrimitiveValueField(id + ".x", "x", value.x);
            value.y = PrimitiveValueField(id + ".y", "y", value.y);
            value.z = PrimitiveValueField(id + ".z", "z", value.z);
            return value;
        }

        public static Vector4 PresentVector4(string id, Vector4 value)
        {
            value.x = PrimitiveValueField(id + ".x", "x", value.x);
            value.y = PrimitiveValueField(id + ".y", "y", value.y);
            value.z = PrimitiveValueField(id + ".z", "z", value.z);
            value.w = PrimitiveValueField(id + ".w", "w", value.w);
            return value;
        }

        public static Quaternion PresentQuaternion(string id, Quaternion value)
        {
            var euler = value.eulerAngles;
            euler.x = PrimitiveValueField(id + ".x", "x", euler.x);
            euler.y = PrimitiveValueField(id + ".y", "y", euler.y);
            euler.z = PrimitiveValueField(id + ".z", "z", euler.z);
            if (euler != value.eulerAngles)
            {
                value = Quaternion.Euler(euler);
            }

            return value;
        }

        public static Color PresentColor(string id, Color value)
        {
            var r = (byte)Mathf.Clamp(value.r * 255.0f, byte.MinValue, byte.MaxValue);
            var g = (byte)Mathf.Clamp(value.g * 255.0f, byte.MinValue, byte.MaxValue);
            var b = (byte)Mathf.Clamp(value.b * 255.0f, byte.MinValue, byte.MaxValue);
            var a = (byte)Mathf.Clamp(value.a * 255.0f, byte.MinValue, byte.MaxValue);

            r = PrimitiveValueField(id + ".r", "r", r);
            g = PrimitiveValueField(id + ".g", "g", g);
            b = PrimitiveValueField(id + ".b", "b", b);
            a = PrimitiveValueField(id + ".a", "a", a);

            value.r = Mathf.Clamp01(r / 255.0f);
            value.g = Mathf.Clamp01(g / 255.0f);
            value.b = Mathf.Clamp01(b / 255.0f);
            value.a = Mathf.Clamp01(a / 255.0f);

            if (GUILayout.Button(string.Empty, GUILayout.Width(72)))
            {
                var picker = ModTools.Instance.ColorPicker;
                if (picker != null)
                {
                    picker.SetColor(value);

                    Vector2 mouse = Input.mousePosition;
                    mouse.y = Screen.height - mouse.y;

                    var windowRect = picker.WindowRect;
                    windowRect.position = mouse;
                    picker.MoveResize(windowRect);
                    picker.Visible = true;
                }
            }

            var lastRect = GUILayoutUtility.GetLastRect();
            lastRect.x += 4.0f;
            lastRect.y += 4.0f;
            lastRect.width -= 8.0f;
            lastRect.height -= 8.0f;
            GUI.DrawTexture(lastRect, ColorPicker.GetColorTexture(id, value), ScaleMode.StretchToFill);

            return value;
        }

        public static T CustomValueField<T>(string id, string name, ValuePresenterDelegate<T> presenter, T value, float ident = 0.0f)
            where T : struct
        {
            GUILayout.BeginHorizontal();

            if (ident != 0.0f)
            {
                GUILayout.Space(ident);
                GUI.contentColor = Config.TypeColor;
                GUILayout.Label(value.GetType().Name);
            }

            GUI.contentColor = Config.NameColor;
            GUILayout.Label(name);

            GUI.contentColor = Config.ValueColor;

            if (ident != 0.0f)
            {
                GUILayout.FlexibleSpace();
            }

            var result = presenter(id, value);

            GUI.contentColor = Color.white;

            GUILayout.EndHorizontal();

            return result;
        }

        private static string BufferedTextField(string id, string value, float fieldSize)
        {
            var focusedFieldId = GUI.GetNameOfFocusedControl();

            if (focusedFieldId != lastFocusedFieldId)
            {
                lastFocusedFieldEmpty = string.IsNullOrEmpty(value);
                lastFocusedFieldId = focusedFieldId;
            }

            if (focusedFieldId == id && lastFocusedFieldEmpty)
            {
                value = string.Empty;
            }

            GUI.SetNextControlName(id);
            var newValue = GUILayout.TextField(value, GUILayout.Width(fieldSize), GUILayout.Height(22f));

            if (focusedFieldId == id)
            {
                lastFocusedFieldEmpty = string.IsNullOrEmpty(newValue);
                if (lastFocusedFieldEmpty)
                {
                    return null;
                }
            }

            return value != newValue ? newValue : null;
        }

        private static float GetTextFieldSize(Type valueType)
        {
            switch (Type.GetTypeCode(valueType))
            {
                case TypeCode.Char:
                    return CharFieldSize;

                case TypeCode.Byte:
                case TypeCode.SByte:
                    return ByteFieldSize;

                case TypeCode.String:
                    return StringFieldSize;

                default:
                    return NumberFieldSize;
            }
        }
    }
}