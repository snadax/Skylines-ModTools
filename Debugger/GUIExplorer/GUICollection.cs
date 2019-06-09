using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModTools.Explorer
{
    public static class GUICollection
    {
        public static void OnSceneTreeReflectICollection(SceneExplorerState state, ReferenceChain refChain, System.Object myProperty)
        {
            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain)) return;

            var collection = myProperty as ICollection;
            if (collection == null)
            {
                return;
            }

            var oldRefChain = refChain;
            var collectionSize = collection.Count;
            if (collectionSize == 0)
            {
                GUILayout.BeginHorizontal();
                GUI.contentColor = Color.yellow;
                GUILayout.Label("Collection is empty!");
                GUI.contentColor = Color.white;
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                return;
            }


            int arrayStart;
            int arrayEnd;
            GUICollectionNavigation.SetUpCollectionNavigation("Collection", state, refChain, oldRefChain, collectionSize, out arrayStart, out arrayEnd);
            int count = 0;
            foreach (var value in collection)
            {
                if (count < arrayStart)
                {
                    count++;
                    continue;
                }

                refChain = oldRefChain.Add(count);
                var type = value.GetType();

                GUILayout.BeginHorizontal();
                SceneExplorerCommon.InsertIndent(refChain.Ident);

                GUIExpander.ExpanderControls(state, refChain, type);

                GUI.contentColor = ModTools.Instance.config.typeColor;

                GUILayout.Label(type.ToString() + " ");

                GUI.contentColor = ModTools.Instance.config.nameColor;

                GUILayout.Label($"{oldRefChain.LastItemName}.[{count}]");

                GUI.contentColor = Color.white;

                GUILayout.Label(" = ");

                GUI.contentColor = ModTools.Instance.config.valueColor;
                GUILayout.Label(value == null ? "null" : value.ToString());

                GUI.contentColor = Color.white;

                GUILayout.FlexibleSpace();
                GUIButtons.SetupButtons(type, value, refChain);
                GUILayout.EndHorizontal();

                if (!TypeUtil.IsSpecialType(type) && state.expandedObjects.ContainsKey(refChain))
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

                count++;
                if (count > arrayEnd)
                {
                    break;
                }
            }
        }
    }
}