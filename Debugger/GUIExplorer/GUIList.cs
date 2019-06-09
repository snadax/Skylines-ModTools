using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModTools.Explorer
{
    public static class GUIList
    {
        public static void OnSceneTreeReflectIList(SceneExplorerState state, 
            ReferenceChain refChain, System.Object myProperty)
        {
            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain)) return;

            var list = myProperty as IList;
            if (list == null)
            {
                return;
            }

            var oldRefChain = refChain;
            var collectionSize = list.Count;
            if (collectionSize == 0)
            {
                GUILayout.BeginHorizontal();
                GUI.contentColor = Color.yellow;
                GUILayout.Label("List is empty!");
                GUI.contentColor = Color.white;
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                return;
            }

            int arrayStart;
            int arrayEnd;
            GUICollectionNavigation.SetUpCollectionNavigation("List", state, refChain, oldRefChain, collectionSize, out arrayStart, out arrayEnd);
            for (int i = arrayStart; i <= arrayEnd; i++)
            {
                refChain = oldRefChain.Add(i);
                if (list[i] == null)
                {
                    continue;
                }

                GUILayout.BeginHorizontal();
                SceneExplorerCommon.InsertIndent(refChain.Ident);


                GUI.contentColor = Color.white;
                var type = list[i] == null ? null : list[i].GetType();
                GUIExpander.ExpanderControls(state, refChain, type);

                GUI.contentColor = ModTools.Instance.config.typeColor;

                GUILayout.Label($"{type} ");

                GUI.contentColor = ModTools.Instance.config.nameColor;

                GUILayout.Label($"{oldRefChain.LastItemName}.[{i}]");

                GUI.contentColor = Color.white;

                GUILayout.Label(" = ");

                GUI.contentColor = ModTools.Instance.config.valueColor;

                if (list[i] == null || !TypeUtil.IsSpecialType(list[i].GetType()))
                {
                    GUILayout.Label(list[i] == null ? "null" : list[i].ToString());
                }
                else
                {
                    try
                    {
                        var newValue = GUIControls.EditorValueField(refChain, refChain.ToString(), list[i].GetType(), list[i]);
                        if (newValue != list[i])
                        {
                            list[i] = newValue;
                        }
                    }
                    catch (Exception)
                    {
                        GUILayout.Label(list[i] == null ? "null" : list[i].ToString());
                    }
                }

                GUI.contentColor = Color.white;

                GUILayout.FlexibleSpace();
                GUIButtons.SetupButtons(type, list[i], refChain);
                GUILayout.EndHorizontal();

                if (!TypeUtil.IsSpecialType(type) && state.expandedObjects.ContainsKey(refChain))
                {
                    if (list[i] is GameObject)
                    {
                        var go = list[i] as GameObject;
                        foreach (var component in go.GetComponents<Component>())
                        {
                            GUIComponent.OnSceneTreeComponent(state, refChain, component);
                        }
                    }
                    else if (list[i] is Transform)
                    {
                        GUITransform.OnSceneTreeReflectUnityEngineTransform(refChain, (Transform)list[i]);
                    }
                    else
                    {
                        GUIReflect.OnSceneTreeReflect(state, refChain, list[i]);
                    }
                }
            }
        }
    }
}