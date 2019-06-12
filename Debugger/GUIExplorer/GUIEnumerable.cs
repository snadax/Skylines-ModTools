using System.Collections;
using UnityEngine;

namespace ModTools.Explorer
{
    public static class GUIEnumerable
    {
        public static void OnSceneTreeReflectIEnumerable(SceneExplorerState state, ReferenceChain refChain, object myProperty)
        {
            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain))
            {
                return;
            }

            if (!(myProperty is IEnumerable enumerable))
            {
                return;
            }

            var count = 0;
            var oldRefChain = refChain;

            foreach (var value in enumerable)
            {
                refChain = oldRefChain.Add(count);

                GUILayout.BeginHorizontal();
                SceneExplorerCommon.InsertIndent(refChain.Ident);

                var type = value?.GetType();
                if (type != null)
                {
                    GUIExpander.ExpanderControls(state, refChain, type);

                    GUI.contentColor = ModTools.Instance.config.typeColor;

                    GUILayout.Label(type.ToString() + " ");
                }

                GUI.contentColor = ModTools.Instance.config.nameColor;

                GUILayout.Label($"{oldRefChain.LastItemName}.[{count}]");

                GUI.contentColor = Color.white;

                GUILayout.Label(" = ");

                GUI.contentColor = ModTools.Instance.config.valueColor;

                GUILayout.Label(value == null ? "null" : value.ToString());

                GUI.contentColor = Color.white;

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                if (type != null && !TypeUtil.IsSpecialType(type) && state.ExpandedObjects.Contains(refChain.UniqueId))
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

                count++;
                if (count >= 128)
                {
                    SceneExplorerCommon.OnSceneTreeMessage(refChain, "Enumerable too large to display");
                    break;
                }
            }
        }
    }
}