using System.Reflection;
using UnityEngine;

namespace ModTools.Explorer
{
    internal static class GUIMemberName
    {
        public static void MemberName(MemberInfo member, int nameHighlightFrom, int nameHighlightLength)
        {
            GUI.contentColor = MainWindow.Instance.Config.NameColor;
            if (nameHighlightFrom == -1)
            {
                GUILayout.Label(member.Name);
            }
            else if (nameHighlightFrom > 0)
            {
                GUILayout.Label(member.Name.Substring(0, nameHighlightFrom));
            }

            if (nameHighlightFrom < 0)
            {
                return;
            }

            GUI.contentColor = MainWindow.Instance.Config.SelectedComponentColor;
            GUILayout.Label(member.Name.Substring(nameHighlightFrom, nameHighlightLength));
            if (nameHighlightFrom + nameHighlightLength >= member.Name.Length)
            {
                return;
            }

            GUI.contentColor = MainWindow.Instance.Config.NameColor;
            GUILayout.Label(member.Name.Substring(nameHighlightFrom + nameHighlightLength));
        }
    }
}