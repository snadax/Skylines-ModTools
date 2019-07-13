using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ModTools.Explorer
{
    internal static class SceneExplorerCommon
    {
        private static readonly Dictionary<int, string> IndentStrings = new Dictionary<int, string>();

        internal static void OnSceneTreeMessage(ReferenceChain refChain, string message)
        {
            GUILayout.BeginHorizontal();
            InsertIndent(refChain.Indentation);
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

            if (!IndentStrings.TryGetValue(indent, out var indentString))
            {
                indentString = new StringBuilder().Insert(0, "· ", indent).ToString();
                IndentStrings.Add(indent, indentString);
            }

            GUILayout.Label(indentString, GUILayout.Width(MainWindow.Instance.Config.TreeIdentSpacing * indent));
        }
    }
}