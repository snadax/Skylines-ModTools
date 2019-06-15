using UnityEngine;

namespace ModTools.UI
{
    internal sealed class GUIArea
    {
        // TODO: convert these fields to properties or refactor the functionality
        public Vector2 AbsolutePosition = Vector2.zero;

        public Vector2 AbsoluteSize = Vector2.zero;

        public Vector2 RelativePosition = Vector2.zero;

        public Vector2 RelativeSize = Vector2.zero;

        private readonly Vector2 margin = new Vector2(8.0f, 8.0f);
        private readonly GUIWindow window;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811", Justification = "Used by Unity components")]
        public GUIArea(GUIWindow window)
        {
            this.window = window;
        }

        public void Begin()
        {
            var position = GetPosition();
            var size = GetSize();
            GUILayout.BeginArea(new Rect(position.x, position.y, size.x, size.y));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822", Justification = "Better logical structure")]
        public void End() => GUILayout.EndArea();

        private Vector2 GetPosition()
            => AbsolutePosition + new Vector2(RelativePosition.x * window.WindowRect.width, RelativePosition.y * window.WindowRect.height) + margin;

        private Vector2 GetSize()
            => AbsoluteSize + new Vector2(RelativeSize.x * window.WindowRect.width, RelativeSize.y * window.WindowRect.height) - margin * 2.0f;
    }
}