using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ModTools.Explorer
{
    public static class SceneExplorerCommon
    {
        private static readonly Dictionary<int, string> indentStrings = new Dictionary<int, string>();

        internal static void OnSceneTreeMessage(ReferenceChain refChain, string message)
        {
            GUILayout.BeginHorizontal();
            InsertIndent(refChain.Ident);
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

        internal static void InsertIndent(int indent)
        {
            if (indent <= 0)
            {
                return;
            }

            if (!indentStrings.TryGetValue(indent, out string indentString))
            {
                indentString = new StringBuilder().Insert(0, "· ", indent).ToString();
                indentStrings.Add(indent, indentString);
            }

            GUILayout.Label(indentString, GUILayout.Width(ModTools.Instance.config.sceneExplorerTreeIdentSpacing * indent));
        }
    }
}