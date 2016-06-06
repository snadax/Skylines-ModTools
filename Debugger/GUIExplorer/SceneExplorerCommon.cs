using UnityEngine;

namespace ModTools.Explorer
{
    public class SceneExplorerCommon
    {
        internal static void OnSceneTreeMessage(ReferenceChain refChain, string message)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(ModTools.Instance.config.sceneExplorerTreeIdentSpacing * refChain.Ident);
            GUILayout.Label(message);
            GUILayout.EndHorizontal();
        }

        internal static bool SceneTreeCheckDepth(ReferenceChain refChain)
        {
            if (refChain.CheckDepth())
            {
                OnSceneTreeMessage(refChain, "Hierarchy too deep, sorry :(");
                return false;
            }

            return true;
        }
    }
}