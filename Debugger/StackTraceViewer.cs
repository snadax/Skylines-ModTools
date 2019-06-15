using System;
using System.Diagnostics;
using ModTools.UI;
using UnityEngine;

namespace ModTools
{
    internal sealed class StackTraceViewer : GUIWindow
    {
        private StackTrace trace;

        private Vector2 scrollPos = Vector2.zero;

        private StackTraceViewer()
            : base("Stack-trace viewer", new Rect(16.0f, 16.0f, 512.0f, 256.0f), Skin)
        {
        }

        private static ModConfiguration Config => MainWindow.Instance.Config;

        public static StackTraceViewer CreateStackTraceViewer(StackTrace trace)
        {
            var go = new GameObject("StackTraceViewer");
            go.transform.parent = MainWindow.Instance.transform;
            var viewer = go.AddComponent<StackTraceViewer>();
            viewer.trace = trace;
            return viewer;
        }

        protected override void OnWindowClosed() => Destroy(this);

        protected override void HandleException(Exception ex) => Logger.Error("Exception in StackTraceViewer - " + ex.Message);

        protected override void DrawWindow()
        {
            if (trace == null)
            {
                return;
            }

            var stackFrames = trace.GetFrames();
            if (stackFrames == null)
            {
                return;
            }

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            var count = 0;
            foreach (var frame in stackFrames)
            {
                GUILayout.BeginHorizontal(Skin.box);
                var method = frame.GetMethod();

                GUILayout.Label(count.ToString(), GUILayout.ExpandWidth(false));

                GUI.contentColor = Config.NameColor;

                GUILayout.Label(method.ToString(), GUILayout.ExpandWidth(false));

                GUI.contentColor = Config.TypeColor;

                if (method.DeclaringType != null)
                {
                    GUILayout.Label(" @ " + method.DeclaringType.ToString(), GUILayout.ExpandWidth(false));
                }

                GUI.contentColor = Color.white;

                GUILayout.EndHorizontal();
                count++;
            }

            GUILayout.EndScrollView();
        }
    }
}