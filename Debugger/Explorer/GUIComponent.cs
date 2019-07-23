using UnityEngine;

namespace ModTools.Explorer
{
    internal static class GUIComponent
    {
        public static void OnSceneTreeComponent(SceneExplorerState state, ReferenceChain refChain, Component component)
        {
            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain))
            {
                return;
            }

            if (component == null)
            {
                SceneExplorerCommon.OnSceneTreeMessage(refChain, "null");
                return;
            }

            GUILayout.BeginHorizontal();
            SceneExplorerCommon.InsertIndent(refChain.Indentation);

            GUI.contentColor = GameObjectUtil.ComponentIsEnabled(component)
                ? MainWindow.Instance.Config.EnabledComponentColor
                : MainWindow.Instance.Config.DisabledComponentColor;

            if (state.CurrentRefChain?.IsSameChain(refChain) != true)
            {
                if (GUILayout.Button(">", GUILayout.ExpandWidth(false)))
                {
                    var sceneExplorer = Object.FindObjectOfType<SceneExplorer>();
                    sceneExplorer.Show(refChain, false);
                }
            }
            else
            {
                GUI.contentColor = MainWindow.Instance.Config.SelectedComponentColor;
                state.PreventCircularReferences.Add(component.gameObject);
                if (GUILayout.Button("<", GUILayout.ExpandWidth(false)))
                {
                    state.CurrentRefChain = null;
                }
            }

            GUILayout.Label(component.GetType().ToString());

            GUI.contentColor = Color.white;
            GUILayout.EndHorizontal();
        }
    }
}