using System;
using UnityEngine;

namespace ModTools.Explorer
{
    public static class GUIRecursiveTree
    {
        public static void OnSceneTreeRecursive(GameObject modToolsGo, SceneExplorerState state, ReferenceChain refChain, GameObject obj)
        {
            if (obj == modToolsGo && !ModTools.DEBUG_MODTOOLS)
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

            if (obj.name == "_ModToolsInternal" && !ModTools.DEBUG_MODTOOLS)
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

                    GUI.contentColor = ModTools.Instance.config.gameObjectColor;
                    GUILayout.Label(obj.name);
                    GUI.contentColor = Color.white;

                    GUILayout.EndHorizontal();

                    var components = obj.GetComponents(typeof(Component));

                    if (ModTools.Instance.config.sceneExplorerSortAlphabetically)
                    {
                        Array.Sort(components, (component, component1) => component.GetType().ToString().CompareTo(component1.GetType().ToString()));
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

                GUI.contentColor = ModTools.Instance.config.gameObjectColor;
                GUILayout.Label(obj.name);
                GUI.contentColor = Color.white;
                GUILayout.EndHorizontal();
            }
        }
    }
}