using System;
using System.Reflection;
using ModTools.UI;
using ModTools.Utils;
using UnityEngine;

namespace ModTools.Explorer
{
    internal static class GUIField
    {
        public static void OnSceneTreeReflectField(SceneExplorerState state, ReferenceChain refChain, object obj, FieldInfo field)
        {
            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain))
            {
                return;
            }

            if (obj == null || field == null)
            {
                SceneExplorerCommon.OnSceneTreeMessage(refChain, "null");
                return;
            }

            GUILayout.BeginHorizontal();
            SceneExplorerCommon.InsertIndent(refChain.Ident);

            GUI.contentColor = Color.white;

            object value = null;

            try
            {
                value = field.GetValue(obj);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            if (value != null)
            {
                GUIExpander.ExpanderControls(state, refChain, field.FieldType);
            }

            if (field.IsInitOnly)
            {
                GUI.enabled = false;
            }

            if (MainWindow.Instance.Config.ShowModifiers)
            {
                GUI.contentColor = MainWindow.Instance.Config.ModifierColor;

                if (field.IsPublic)
                {
                    GUILayout.Label("public ");
                }
                else if (field.IsPrivate)
                {
                    GUILayout.Label("private ");
                }

                GUI.contentColor = MainWindow.Instance.Config.MemberTypeColor;

                GUILayout.Label("field ");

                if (field.IsStatic)
                {
                    GUI.contentColor = MainWindow.Instance.Config.KeywordColor;
                    GUILayout.Label("static ");
                }

                if (field.IsInitOnly)
                {
                    GUI.contentColor = MainWindow.Instance.Config.KeywordColor;
                    GUILayout.Label("const ");
                }
            }

            GUI.contentColor = MainWindow.Instance.Config.TypeColor;
            GUILayout.Label(field.FieldType + " ");

            GUI.contentColor = MainWindow.Instance.Config.NameColor;

            GUILayout.Label(field.Name);

            GUI.contentColor = Color.white;
            GUILayout.Label(" = ");
            GUI.contentColor = MainWindow.Instance.Config.ValueColor;

            if (value == null || !TypeUtil.IsSpecialType(field.FieldType))
            {
                GUILayout.Label(value?.ToString() ?? "null");
            }
            else
            {
                try
                {
                    var newValue = GUIControls.EditorValueField(refChain.UniqueId, field.FieldType, value);
                    if (!newValue.Equals(value))
                    {
                        field.SetValue(obj, newValue);
                    }
                }
                catch (Exception)
                {
                    GUILayout.Label(value.ToString());
                }
            }

            GUI.enabled = true;
            GUI.contentColor = Color.white;

            GUILayout.FlexibleSpace();

            GUIButtons.SetupButtons(refChain, value, valueIndex: 0, field.Name);
            object paste = null;
            var doPaste = !field.IsLiteral && !field.IsInitOnly;
            if (doPaste)
            {
                doPaste = GUIButtons.SetupPasteButon(field.FieldType, out paste);
            }

            GUILayout.EndHorizontal();
            if (value != null && !TypeUtil.IsSpecialType(field.FieldType) && state.ExpandedObjects.Contains(refChain.UniqueId))
            {
                if (value is GameObject go)
                {
                    foreach (var component in go.GetComponents<Component>())
                    {
                        GUIComponent.OnSceneTreeComponent(state, refChain, component);
                    }
                }
                else if (value is Transform transform)
                {
                    GUITransform.OnSceneTreeReflectUnityEngineTransform(refChain, transform);
                }
                else
                {
                    GUIReflect.OnSceneTreeReflect(state, refChain, value);
                }
            }

            if (doPaste)
            {
                try
                {
                    field.SetValue(obj, paste);
                }
                catch (Exception e)
                {
                    Logger.Warning(e.Message);
                }
            }
        }
    }
}