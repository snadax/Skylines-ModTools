using System;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

namespace ModTools.Explorer
{
    public class GUIProperty
    {
        public static void OnSceneTreeReflectProperty(SceneExplorerState state, ReferenceChain refChain, System.Object obj, PropertyInfo property)
        {
            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain))
                return;

            if (obj == null || property == null)
            {
                SceneExplorerCommon.OnSceneTreeMessage(refChain, "null");
                return;
            }

            GUILayout.BeginHorizontal();
            SceneExplorerCommon.InsertIndent(refChain.Ident);

            bool propertyWasEvaluated = false;
            object value = null;

            Exception exceptionOnGetting = null;

            if (property.CanRead && ModTools.Instance.config.sceneExplorerEvaluatePropertiesAutomatically || state.EvaluatedProperties.Contains(refChain.UniqueId))
            {
                try
                {
                    value = property.GetValue(obj, null);
                    propertyWasEvaluated = true;
                }
                catch (Exception e)
                {
                    exceptionOnGetting = e;
                }

                if (value != null && exceptionOnGetting == null)
                {
                    GUIExpander.ExpanderControls(state, refChain, property.PropertyType, obj);
                }
            }

            GUI.contentColor = Color.white;

            if (!property.CanWrite)
            {
                GUI.enabled = false;
            }

            if (ModTools.Instance.config.sceneExplorerShowModifiers)
            {
                GUI.contentColor = ModTools.Instance.config.memberTypeColor;
                GUILayout.Label("property ");

                if (!property.CanWrite)
                {
                    GUI.contentColor = ModTools.Instance.config.keywordColor;
                    GUILayout.Label("const ");
                }
            }

            GUI.contentColor = ModTools.Instance.config.typeColor;

            GUILayout.Label(property.PropertyType.ToString() + " ");

            GUI.contentColor = ModTools.Instance.config.nameColor;

            GUILayout.Label(property.Name);

            GUI.contentColor = Color.white;
            GUILayout.Label(" = ");
            GUI.contentColor = ModTools.Instance.config.valueColor;

            if (!ModTools.Instance.config.sceneExplorerEvaluatePropertiesAutomatically && !state.EvaluatedProperties.Contains(refChain.UniqueId))
            {
                GUI.enabled = true;

                if (GUILayout.Button("Evaluate"))
                {
                    state.EvaluatedProperties.Add(refChain.UniqueId);
                }
            }
            else
            {
                if (!propertyWasEvaluated && property.CanRead)
                {
                    try
                    {
                        value = property.GetValue(obj, null);
                        propertyWasEvaluated = true;
                    }
                    catch (Exception e)
                    {
                        exceptionOnGetting = e;
                    }
                }
                if (exceptionOnGetting != null)
                {
                    GUI.contentColor = Color.red;
                    GUILayout.Label("Exception happened when getting property value");
                    GUI.contentColor = Color.white;
                    GUI.enabled = true;
                    GUIStackTrace.StackTraceButton(new StackTrace(exceptionOnGetting, true));

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    return;
                }

                if (value == null || !TypeUtil.IsSpecialType(property.PropertyType))
                {
                    if (property.CanRead)
                    {
                        GUILayout.Label(value == null ? "null" : value.ToString());
                    }
                    else
                    {
                        GUILayout.Label("(no get method)");
                    }

                    GUI.contentColor = Color.white;
                }
                else
                {
                    try
                    {
                        var newValue = GUIControls.EditorValueField(refChain, refChain.UniqueId, property.PropertyType, value);
                        if (newValue != value)
                        {
                            property.SetValue(obj, newValue, null);
                        }
                    }
                    catch (Exception)
                    {
                        if (property.CanRead)
                        {
                            GUILayout.Label(value == null ? "null" : value.ToString());
                        }
                        else
                        {
                            GUILayout.Label("(no get method)");
                        }

                        GUI.contentColor = Color.white;
                    }
                }
            }

            GUI.enabled = true;
            GUI.contentColor = Color.white;

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Watch"))
            {
                ModTools.Instance.watches.AddWatch(refChain);
            }
            GUIButtons.SetupButtons(property.PropertyType, value, refChain);
            object paste = null;
            var doPaste = property.CanWrite;
            if (doPaste)
            {
                doPaste = GUIButtons.SetupPasteButon(property.PropertyType, out paste);
            }
            GUILayout.EndHorizontal();

            if (value != null && state.ExpandedObjects.Contains(refChain.UniqueId))
            {
                if (value is GameObject)
                {
                    var go = value as GameObject;
                    foreach (var component in go.GetComponents<Component>())
                    {
                        GUIComponent.OnSceneTreeComponent(state, refChain, component);
                    }
                }
                else if (value is Transform)
                {
                    GUITransform.OnSceneTreeReflectUnityEngineTransform(refChain, (Transform)value);
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
                    property.SetValue(obj, paste, null);
                }
                catch (Exception e)
                {
                    Log.Warning(e.Message);
                }
            }
        }
    }
}