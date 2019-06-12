using ModTools.Utils;
using UnityEngine;

namespace ModTools
{
    public sealed class TextureViewer : GUIWindow
    {
        public Texture previewTexture;
        public ReferenceChain caller;

        private TextureViewer() : base("Texture Viewer", new Rect(512, 128, 512, 512), skin)
        {
            onDraw = DrawWindow;
            onClose = HandleClose;
        }

        public static TextureViewer CreateTextureViewer(ReferenceChain refChain, Texture texture)
        {
            var go = new GameObject("TextureViewer");
            go.transform.parent = ModTools.Instance.transform;
            var textureViewer = go.AddComponent<TextureViewer>();
            textureViewer.caller = refChain;
            var texture3D = texture as Texture3D;
            textureViewer.previewTexture = texture3D == null ? texture : texture3D.ToTexture2D();
            textureViewer.visible = true;
            return textureViewer;
        }

        private void HandleClose() => Destroy(this);

        private void DrawWindow()
        {
            if (previewTexture != null)
            {
                title = $"Previewing \"{previewTexture.name}\"";

                if (GUILayout.Button("Dump .png", GUILayout.Width(128)))
                {
                    TextureUtil.DumpTextureToPNG(previewTexture);
                }

                var aspect = previewTexture.width / (previewTexture.height + 60.0f);
                rect.width = rect.height * aspect;
                GUI.DrawTexture(new Rect(0.0f, 60.0f, rect.width, rect.height - 60.0f), previewTexture, ScaleMode.ScaleToFit, false);
            }
            else
            {
                title = "Texture Viewer";
                GUILayout.Label("Use the Scene Explorer to select a Texture for preview");
            }
        }
    }
}