using System;
using System.Collections;
using UnityEngine;

namespace ModTools.Explorer
{
    public static class GUIList
    {
        public static void OnSceneTreeReflectIList(SceneExplorerState state, ReferenceChain refChain, object myProperty)
        {
            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain))
            {
                return;
            }

            if (!(myProperty is IList list))
            {
                return;
            }

            ReferenceChain oldRefChain = refChain;
            int collectionSize = list.Count;
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

            Type listItemType = list.GetType().GetElementType();
            System.Reflection.FieldInfo flagsField = listItemType?.GetField("m_flags");
            bool flagIsEnum = flagsField?.FieldType.IsEnum == true && Type.GetTypeCode(flagsField.FieldType) == TypeCode.Int32;

            GUICollectionNavigation.SetUpCollectionNavigation("List", state, refChain, oldRefChain, collectionSize, out int arrayStart, out int arrayEnd);
            for (int i = arrayStart; i <= arrayEnd; i++)
            {
                refChain = oldRefChain.Add(i);

                GUILayout.BeginHorizontal();
                SceneExplorerCommon.InsertIndent(refChain.Ident);

                GUI.contentColor = Color.white;

                object value = list[i];
                Type type = value?.GetType() ?? listItemType;
                bool isNullOrEmpty = value == null || flagIsEnum && Convert.ToInt32(flagsField.GetValue(value)) == 0;
                if (type != null)
                {
                    if (!isNullOrEmpty)
                    {
                        GUIExpander.ExpanderControls(state, refChain, type);
                    }

                    GUI.contentColor = ModTools.Instance.config.typeColor;

                    GUILayout.Label($"{type} ");
                }

                GUI.contentColor = ModTools.Instance.config.nameColor;

                GUILayout.Label($"{oldRefChain.LastItemName}.[{i}]");

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
                        foreach (Component component in go.GetComponents<Component>())
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
            }
        }
    }
}