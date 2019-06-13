using System;
using System.Collections.Generic;
using UnityEngine;

namespace ModTools.UI
{
    internal sealed class ColorPicker : GUIWindow
    {
        private static readonly Dictionary<string, Texture2D> TextureCache = new Dictionary<string, Texture2D>();
        private static readonly Color LineColor = Color.white;

        private readonly int colorPickerSize = 142;
        private readonly int huesBarWidth = 26;

        private Texture2D colorPickerTexture;
        private Texture2D huesBarTexture;
        private ColorUtil.HSV currentHSV;
        private float originalAlpha;

        private Rect colorPickerRect;
        private Rect huesBarRect;

        private Texture2D lineTexTexture;

        public ColorPicker()
            : base("ColorPicker", new Rect(16.0f, 16.0f, 188.0f, 156.0f), Skin)
        {
            Resizable = false;
            HasTitlebar = false;
            HasCloseButton = false;

            colorPickerRect = new Rect(8.0f, 8.0f, colorPickerSize, colorPickerSize);
            huesBarRect = new Rect(colorPickerRect.x + colorPickerSize + 4.0f, colorPickerRect.y, huesBarWidth, colorPickerRect.height);
            Visible = false;
        }

        private Texture2D Texture => colorPickerTexture ?? (colorPickerTexture = new Texture2D(colorPickerSize, colorPickerSize));

        private Texture2D HuesBar => huesBarTexture ?? (huesBarTexture = DrawHuesBar(huesBarWidth, colorPickerSize));

        private Texture2D LineTex => lineTexTexture ?? (lineTexTexture = DrawLineTex());

        public static Texture2D GetColorTexture(string id, Color color)
        {
            if (!TextureCache.TryGetValue(id, out var texture))
            {
                texture = new Texture2D(1, 1);
                TextureCache.Add(id, texture);
            }

            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        public static void DrawColorPicker(Texture2D texture, double hue)
        {
            for (var x = 0; x < texture.width; x++)
            {
                for (var y = 0; y < texture.height; y++)
                {
                    texture.SetPixel(x, y, GetColorAtXY(hue, x / (float)texture.width, 1.0f - y / (float)texture.height));
                }
            }

            texture.Apply();
        }

        public static Texture2D DrawHuesBar(int width, int height)
        {
            var texture = new Texture2D(width, height);

            for (var y = 0; y < height; y++)
            {
                var color = GetColorAtT(y / (float)height * 360.0f);

                for (var x = 0; x < width; x++)
                {
                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            return texture;
        }

        public static Texture2D DrawLineTex()
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, LineColor);
            tex.Apply();
            return tex;
        }

        public static Color GetColorAtT(double hue)
            => ColorUtil.HSV.HSV2RGB(new ColorUtil.HSV { H = hue, S = 1.0f, V = 1.0f });

        public static Color GetColorAtXY(double hue, float xT, float yT)
            => ColorUtil.HSV.HSV2RGB(new ColorUtil.HSV { H = hue, S = xT, V = yT });

        public void SetColor(Color color)
        {
            originalAlpha = color.a;
            currentHSV = ColorUtil.HSV.RGB2HSV(color);
            currentHSV.H = 360.0f - currentHSV.H;
            RedrawPicker();
        }

        public void Update()
        {
            Vector2 mouse = Input.mousePosition;
            mouse.y = Screen.height - mouse.y;

            if (Input.GetMouseButton(0) && !WindowRect.Contains(mouse))
            {
                Visible = false;
                return;
            }

            mouse -= WindowRect.position;

            if (Input.GetMouseButton(0))
            {
                if (huesBarRect.Contains(mouse))
                {
                    currentHSV.H = (1.0f - Mathf.Clamp01((mouse.y - huesBarRect.y) / huesBarRect.height)) * 360.0f;
                    RedrawPicker();

                    // TODO: color changing
                    ////InternalOnColorChanged(ColorUtil.HSV.HSV2RGB(currentHSV));
                }

                if (colorPickerRect.Contains(mouse))
                {
                    currentHSV.S = Mathf.Clamp01((mouse.x - colorPickerRect.x) / colorPickerRect.width);
                    currentHSV.V = Mathf.Clamp01((mouse.y - colorPickerRect.y) / colorPickerRect.height);

                    // TODO: color changing
                    ////InternalOnColorChanged(ColorUtil.HSV.HSV2RGB(currentHSV));
                }
            }
        }

        protected override void HandleException(Exception ex)
        {
            Log.Error("Exception in ColorPicker - " + ex.Message);
            Visible = false;
        }

        protected override void DrawWindow()
        {
            GUI.DrawTexture(colorPickerRect, Texture);
            GUI.DrawTexture(huesBarRect, HuesBar);

            var huesBarLineY = huesBarRect.y + (1.0f - (float)currentHSV.H / 360.0f) * huesBarRect.height;
            GUI.DrawTexture(new Rect(huesBarRect.x - 2.0f, huesBarLineY, huesBarRect.width + 4.0f, 2.0f), LineTex);

            var colorPickerLineY = colorPickerRect.x + (float)currentHSV.V * colorPickerRect.width;
            GUI.DrawTexture(new Rect(colorPickerRect.x - 1.0f, colorPickerLineY, colorPickerRect.width + 2.0f, 1.0f), LineTex);

            var colorPickerLineX = colorPickerRect.y + (float)currentHSV.S * colorPickerRect.height;
            GUI.DrawTexture(new Rect(colorPickerLineX, colorPickerRect.y - 1.0f, 1.0f, colorPickerRect.height + 2.0f), LineTex);
        }

        private void RedrawPicker() => DrawColorPicker(Texture, currentHSV.H);
    }
}