using System.Diagnostics;
using UnityEngine;

namespace ModTools
{
    public static class GUIStackTrace
    {
        public static void StackTraceButton(StackTrace stackTrace)
        {
            if (GUILayout.Button("Stack trace", GUILayout.ExpandWidth(false)))
            {
                var viewer = StackTraceViewer.CreateStackTraceViewer(stackTrace);
                Vector3 mouse = Input.mousePosition;
                mouse.y = Screen.height - mouse.y;
                viewer.rect.position = mouse;
                viewer.visible = true;
            }
        }
    }
}