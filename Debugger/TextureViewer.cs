using ModTools.UI;
using ModTools.Utils;
using UnityEngine;

namespace ModTools
{
    internal sealed class TextureViewer : GUIWindow
    {
        private Texture previewTexture;

        private TextureViewer()
            : base("Texture Viewer", new Rect(512, 128, 512, 512), Skin)
        {
        }

        public static TextureViewer CreateTextureViewer(Texture texture)
        {
            var go = new GameObject("TextureViewer");
            go.transform.parent = MainWindow.Instance.transform;
            var textureViewer = go.AddComponent<TextureViewer>();
            var texture3D = texture as Texture3D;
            textureViewer.previewTexture = texture3D == null ? texture : texture3D.ToTexture2D();
            textureViewer.Visible = true;
            return textureViewer;
        }

        protected override void OnWindowClosed() => Destroy(this);

        protected override void DrawWindow()
        {
            if (previewTexture == null)
            {
                Title = "Texture Viewer";
                GUILayout.Label("Use the Scene Explorer to select a Texture for preview");
                return;
            }

            Title = $"Previewing \"{previewTexture.name}\"";

            if (GUILayout.Button("Dump .png", GUILayout.Width(128)))
            {
                TextureUtil.DumpTextureToPNG(previewTexture);
            }

            var aspect = previewTexture.width / (previewTexture.height + 60.0f);
            var newWindowRect = WindowRect;

            newWindowRect.width = newWindowRect.height * aspect;
            MoveResize(newWindowRect);

            GUI.DrawTexture(new Rect(0.0f, 60.0f, WindowRect.width, WindowRect.height - 60.0f), previewTexture, ScaleMode.ScaleToFit, false);
        }
    }
}