using System.Diagnostics;
using ModTools.Console;
using UnityEngine;

namespace ModTools.UI
{
    internal static class GUIStackTrace
    {
        public static void StackTraceButton(StackTrace stackTrace, string errorMessage = "")
        {
            if (GUILayout.Button("Stack trace", GUILayout.ExpandWidth(false)))
            {
                var viewer = StackTraceViewer.CreateStackTraceViewer(stackTrace, errorMessage);
                var mouse = Input.mousePosition;
                mouse.y = Screen.height - mouse.y;

                var windowRect = viewer.WindowRect;
                windowRect.position = mouse;
                viewer.MoveResize(windowRect);

                viewer.Visible = true;
            }
        }
    }
}