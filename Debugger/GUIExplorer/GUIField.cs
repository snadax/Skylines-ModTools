using System;
using System.Reflection;
using UnityEngine;

namespace ModTools.Explorer
{
    public static class GUIField
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

            if (ModTools.Instance.config.sceneExplorerShowModifiers)
            {
                GUI.contentColor = ModTools.Instance.config.modifierColor;

                if (field.IsPublic)
                {
                    GUILayout.Label("public ");
                }
                else if (field.IsPrivate)
                {
                    GUILayout.Label("private ");
                }

                GUI.contentColor = ModTools.Instance.config.memberTypeColor;

                GUILayout.Label("field ");

                if (field.IsStatic)
                {
                    GUI.contentColor = ModTools.Instance.config.keywordColor;
                    GUILayout.Label("static ");
                }

                if (field.IsInitOnly)
                {
                    GUI.contentColor = ModTools.Instance.config.keywordColor;
                    GUILayout.Label("const ");
                }
            }

            GUI.contentColor = ModTools.Instance.config.typeColor;
            GUILayout.Label(field.FieldType + " ");

            GUI.contentColor = ModTools.Instance.config.nameColor;

            GUILayout.Label(field.Name);

            GUI.contentColor = Color.white;
            GUILayout.Label(" = ");
            GUI.contentColor = ModTools.Instance.config.valueColor;

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
            if (GUILayout.Button("Watch"))
            {
                ModTools.Instance.watches.AddWatch(refChain);
            }
            GUIButtons.SetupButtons(field.FieldType, value, refChain);
            object paste = null;
            var doPaste = !field.IsLiteral && !field.IsInitOnly;
            if (doPaste)
            {
                doPaste = GUIButtons.SetupPasteButon(field.FieldType, out paste);
            }
            GUILayout.EndHorizontal();
            if (value != null && !TypeUtil.IsSpecialType(field.FieldType) && state.ExpandedObjects.Contains(refChain.UniqueId))
            {
                if (value is GameObject)
                {
                    var go = value as GameObject;
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
                    Log.Warning(e.Message);
                }
            }
        }
    }
}