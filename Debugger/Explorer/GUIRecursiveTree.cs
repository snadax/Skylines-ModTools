using System;
using UnityEngine;
using Object = UnityEngine.Object;

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

                    var currentlySelected = state.CurrentRefChain?.IsSameChain(refChain) == true;
                    GUI.contentColor = currentlySelected
                        ? MainWindow.Instance.Config.SelectedComponentColor
                        : obj.activeInHierarchy ? MainWindow.Instance.Config.GameObjectColor : MainWindow.Instance.Config.DisabledComponentColor;

                    GUILayout.Label(obj.name);
                    GUI.contentColor = Color.white;

                    if (currentlySelected)
                    {
                        if (GUILayout.Button("<", GUILayout.ExpandWidth(false)))
                        {
                            state.CurrentRefChain = null;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(">", GUILayout.ExpandWidth(false)))
                        {
                            var sceneExplorer = Object.FindObjectOfType<SceneExplorer>();
                            sceneExplorer.Show(refChain, false);
                        }
                    }

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

                var currentlySelected = state.CurrentRefChain?.IsSameChain(refChain) == true;
                GUI.contentColor = currentlySelected
                    ? MainWindow.Instance.Config.SelectedComponentColor
                    : obj.activeInHierarchy ? MainWindow.Instance.Config.GameObjectColor : MainWindow.Instance.Config.DisabledComponentColor;
                GUILayout.Label(obj.name);
                GUI.contentColor = Color.white;
                GUILayout.EndHorizontal();
            }
        }
    }
}