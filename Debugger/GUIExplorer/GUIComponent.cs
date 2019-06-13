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
            SceneExplorerCommon.InsertIndent(refChain.Ident);

            GUI.contentColor = Util.ComponentIsEnabled(component)
                ? ModTools.Instance.config.EnabledComponentColor
                : ModTools.Instance.config.DisabledComponentColor;

            if (state.CurrentRefChain?.Equals(refChain.Add(component)) != true)
            {
                if (GUILayout.Button(">", GUILayout.ExpandWidth(false)))
                {
                    state.CurrentRefChain = refChain.Add(component);
                    state.CurrentRefChain.IdentOffset = -(refChain.Length + 1);
                }
            }
            else
            {
                GUI.contentColor = ModTools.Instance.config.SelectedComponentColor;
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