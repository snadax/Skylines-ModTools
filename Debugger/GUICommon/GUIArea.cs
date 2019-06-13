using UnityEngine;

namespace ModTools
{
    internal sealed class GUIArea
    {
        public Vector2 AbsolutePosition = Vector2.zero;

        public Vector2 AbsoluteSize = Vector2.zero;

        public Vector2 RelativePosition = Vector2.zero;

        public Vector2 RelativeSize = Vector2.zero;

        private readonly Vector2 margin = new Vector2(8.0f, 8.0f);
        private readonly GUIWindow window;

        public GUIArea(GUIWindow window)
        {
            this.window = window;
        }

        private Vector2 GetPosition()
            => AbsolutePosition + new Vector2(RelativePosition.x * window.WindowRect.width, RelativePosition.y * window.WindowRect.height) + margin;

        private Vector2 GetSize()
            => AbsoluteSize + new Vector2(RelativeSize.x * window.WindowRect.width, RelativeSize.y * window.WindowRect.height) - margin * 2.0f;

        public void Begin()
        {
            var position = GetPosition();
            var size = GetSize();
            GUILayout.BeginArea(new Rect(position.x, position.y, size.x, size.y));
        }

        public void End() => GUILayout.EndArea();
    }
}