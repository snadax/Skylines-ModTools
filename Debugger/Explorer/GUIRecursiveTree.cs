using System;
using UnityEngine;

namespace ModTools.Explorer
{
    internal static class GUIRecursiveTree
    {
        public static void OnSceneTreeRecursive(GameObject modToolsGo, SceneExplorerState state, ReferenceChain refChain, GameObject obj)
        {
            if (obj == modToolsGo && !ModTools.DEBUGMODTOOLS)
            {
                return;
            }

            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain))
            {
                return;
            }

            if (obj == null)
            {
                SceneExplorerCommon.OnSceneTreeMessage(refChain, "null");
                return;
            }

            if (obj.name == "_ModToolsInternal" && !ModTools.DEBUGMODTOOLS)
            {
                return;
            }

            if (state.ExpandedGameObjects.Contains(refChain.UniqueId))
            {
                try
                {
                    GUILayout.BeginHorizontal();
                    SceneExplorerCommon.InsertIndent(refChain.Ident);

                    if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
                    {
                        state.ExpandedGameObjects.Remove(refChain.UniqueId);
                    }

                    GUI.contentColor = ModTools.Instance.Config.GameObjectColor;
                    GUILayout.Label(obj.name);
                    GUI.contentColor = Color.white;

                    GUILayout.EndHorizontal();

                    var components = obj.GetComponents(typeof(Component));

                    if (ModTools.Instance.Config.SortItemsAlphabetically)
                    {
                        Array.Sort(components, (x, y) => string.CompareOrdinal(x.GetType().ToString(), y.GetType().ToString()));
                    }

                    foreach (var component in components)
                    {
                        GUIComponent.OnSceneTreeComponent(state, refChain.Add(component), component);
                    }

                    for (var i = 0; i < obj.transform.childCount; i++)
                    {
                        OnSceneTreeRecursive(modToolsGo, state, refChain.Add(obj.transform.GetChild(i)), obj.transform.GetChild(i).gameObject);
                    }
                }
                catch (Exception)
                {
                    state.ExpandedGameObjects.Remove(refChain.UniqueId);
                    throw;
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                SceneExplorerCommon.InsertIndent(refChain.Ident);

                if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                {
                    state.ExpandedGameObjects.Add(refChain.UniqueId);
                }

                GUI.contentColor = ModTools.Instance.Config.GameObjectColor;
                GUILayout.Label(obj.name);
                GUI.contentColor = Color.white;
                GUILayout.EndHorizontal();
            }
        }
    }
}