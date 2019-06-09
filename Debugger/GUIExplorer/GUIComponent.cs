using UnityEngine;

namespace ModTools.Explorer
{
    public static class GUIComponent
    {
        public static void OnSceneTreeComponent(SceneExplorerState state, ReferenceChain refChain, Component component)
        {
            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain)) return;

            if (component == null)
            {
                SceneExplorerCommon.OnSceneTreeMessage(refChain, "null");
                return;
            }

            GUILayout.BeginHorizontal();
            SceneExplorerCommon.InsertIndent(refChain.Ident);

            if (Util.ComponentIsEnabled(component))
            {
                GUI.contentColor = ModTools.Instance.config.enabledComponentColor;
            }
            else
            {
                GUI.contentColor = ModTools.Instance.config.disabledComponentColor;
            }

            if (state.currentRefChain == null || !state.currentRefChain.Equals(refChain.Add(component)))
            {
                if (GUILayout.Button(">", GUILayout.ExpandWidth(false)))
                {
                    state.currentRefChain = refChain.Add(component);
                    state.currentRefChain.IdentOffset = -(refChain.Length + 1);
                }
            }
            else
            {
                GUI.contentColor = ModTools.Instance.config.selectedComponentColor;
                if (GUILayout.Button("<", GUILayout.ExpandWidth(false)))
                {
                    state.currentRefChain = null;
                }
            }

            GUILayout.Label(component.GetType().ToString());

            GUI.contentColor = Color.white;
            GUILayout.EndHorizontal();
        }
    }
}