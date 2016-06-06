using System;
using UnityEngine;

namespace ModTools.Explorer
{
    public class GUIRecursiveTree
    {
        public static void OnSceneTreeRecursive(GameObject modToolsGo, SceneExplorerState state, ReferenceChain refChain, GameObject obj)
        {
            if (obj == modToolsGo && !ModTools.DEBUG_MODTOOLS)
            {
                return;
            }

            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain)) return;

            if (obj == null)
            {
                SceneExplorerCommon.OnSceneTreeMessage(refChain, "null");
                return;
            }

            if (obj.name == "_ModToolsInternal" && !ModTools.DEBUG_MODTOOLS)
            {
                return;
            }

            if (state.expandedGameObjects.ContainsKey(refChain))
            {
                try
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(ModTools.Instance.config.sceneExplorerTreeIdentSpacing * refChain.Ident);

                    if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
                    {
                        state.expandedGameObjects.Remove(refChain);
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

                    for (int i = 0; i < obj.transform.childCount; i++)
                    {
                        OnSceneTreeRecursive(modToolsGo, state, refChain.Add(obj.transform.GetChild(i)), obj.transform.GetChild(i).gameObject);
                    }
                }
                catch (Exception)
                {
                    state.expandedGameObjects.Remove(refChain);
                    throw;
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(ModTools.Instance.config.sceneExplorerTreeIdentSpacing * refChain.Ident);

                if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                {
                    state.expandedGameObjects.Add(refChain, true);
                }

                GUI.contentColor = ModTools.Instance.config.gameObjectColor;
                GUILayout.Label(obj.name);
                GUI.contentColor = Color.white;
                GUILayout.EndHorizontal();
            }
        }
    }
}