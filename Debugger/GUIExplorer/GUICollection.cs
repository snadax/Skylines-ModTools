using System;
using System.Collections;
using UnityEngine;

namespace ModTools.Explorer
{
    public static class GUICollection
    {
        public static void OnSceneTreeReflectICollection(SceneExplorerState state, ReferenceChain refChain, System.Object myProperty)
        {
            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain))
                return;

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

            var collectionItemType = collection.GetType().GetElementType();
            var flagsField = collectionItemType?.GetField("m_flags");
            bool flagIsEnum = flagsField?.FieldType.IsEnum == true && Type.GetTypeCode(flagsField.FieldType) == TypeCode.Int32;

            GUICollectionNavigation.SetUpCollectionNavigation("Collection", state, refChain, oldRefChain, collectionSize, out int arrayStart, out int arrayEnd);
            int count = 0;
            foreach (var value in collection)
            {
                if (count < arrayStart)
                {
                    count++;
                    continue;
                }

                refChain = oldRefChain.Add(count);

                GUILayout.BeginHorizontal();
                SceneExplorerCommon.InsertIndent(refChain.Ident);

                bool isNullOrEmpty = value == null || flagIsEnum && Convert.ToInt32(flagsField.GetValue(value)) == 0;

                var type = value?.GetType() ?? collectionItemType;
                if (type != null)
                {
                    if (!isNullOrEmpty)
                    {
                        GUIExpander.ExpanderControls(state, refChain, type);
                    }

                    GUI.contentColor = ModTools.Instance.config.typeColor;

                    GUILayout.Label(type.ToString() + " ");
                }

                GUI.contentColor = ModTools.Instance.config.nameColor;

                GUILayout.Label($"{oldRefChain.LastItemName}.[{count}]");

                GUI.contentColor = Color.white;

                GUILayout.Label(" = ");

                GUI.contentColor = ModTools.Instance.config.valueColor;
                GUILayout.Label(value == null ? "null" : isNullOrEmpty ? "empty" : value.ToString());

                GUI.contentColor = Color.white;

                GUILayout.FlexibleSpace();

                if (!isNullOrEmpty)
                {
                    GUIButtons.SetupButtons(type, value, refChain);
                }

                GUILayout.EndHorizontal();

                if (!isNullOrEmpty && !TypeUtil.IsSpecialType(type) && state.ExpandedObjects.Contains(refChain.UniqueId))
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