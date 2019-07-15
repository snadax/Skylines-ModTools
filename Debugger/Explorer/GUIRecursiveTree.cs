using System;
using UnityEngine;

namespace ModTools.Explorer
{
    internal static class GUIRecursiveTree
    {
        public static void OnSceneTreeRecursive(GameObject modToolsGo, SceneExplorerState state, ReferenceChain refChain, GameObject obj)
        {
#if !DEBUG
            if (obj == modToolsGo)
            {
                return;
            }
#endif

            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain))
            {
                return;
            }

            if (obj == null)
            {
                SceneExplorerCommon.OnSceneTreeMessage(refChain, "null");
                return;
            }

            if (state.ExpandedGameObjects.Contains(refChain.UniqueId))
            {
                try
                {
                    GUILayout.BeginHorizontal();
                    SceneExplorerCommon.InsertIndent(refChain.Indentation);

                    if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
                    {
                        state.ExpandedGameObjects.Remove(refChain.UniqueId);
                    }

                    GUI.contentColor = state.CurrentRefChain?.IsSameChain(refChain) == true ? MainWindow.Instance.Config.SelectedComponentColor : MainWindow.Instance.Config.GameObjectColor;

                    GUILayout.Label(obj.name);
                    GUI.contentColor = Color.white;

                    GUILayout.EndHorizontal();

                    var gameObjects = new GameObject[obj.transform.childCount];
                    for (var i = 0; i < obj.transform.childCount; i++)
                    {
                        gameObjects[i] = obj.transform.GetChild(i).gameObject;
                    }
                    
                    if (MainWindow.Instance.Config.SortItemsAlphabetically)
                    {
                        Array.Sort(gameObjects, (x, y) => string.CompareOrdinal(x?.name, y?.name));
                    }
                    
                    foreach (var gameObject in gameObjects)
                    {
                        OnSceneTreeRecursive(modToolsGo, state, refChain.Add(gameObject), gameObject);                        
                    }
                    
                    var components = obj.GetComponents(typeof(Component));

                    if (MainWindow.Instance.Config.SortItemsAlphabetically)
                    {
                        Array.Sort(components, (x, y) => string.CompareOrdinal(x.GetType().ToString(), y.GetType().ToString()));
                    }

                    foreach (var component in components)
                    {
                        GUIComponent.OnSceneTreeComponent(state, refChain.Add(component), component);
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
                SceneExplorerCommon.InsertIndent(refChain.Indentation);

                if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
                {
                    state.ExpandedGameObjects.Add(refChain.UniqueId);
                }

                GUI.contentColor = state.CurrentRefChain?.IsSameChain(refChain) == true ? MainWindow.Instance.Config.SelectedComponentColor : MainWindow.Instance.Config.GameObjectColor;
                GUILayout.Label(obj.name);
                GUI.contentColor = Color.white;
                GUILayout.EndHorizontal();
            }
        }
    }
}