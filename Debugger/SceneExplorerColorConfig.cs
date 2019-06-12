using System;
using UnityEngine;

namespace ModTools
{
    public class SceneExplorerColorConfig : GUIWindow
    {
        private static Configuration config => ModTools.Instance.config;

        private readonly string[] availableFonts;
        private int selectedFont;

        public SceneExplorerColorConfig() : base("Font/ color configuration", new Rect(16.0f, 16.0f, 600.0f, 490.0f), skin)
        {
            onDraw = DrawWindow;
            onException = HandleException;
            visible = false;
            resizable = false;

            availableFonts = Font.GetOSInstalledFontNames();
            var c = 0;
            var configFont = config.fontName;

            foreach (var font in availableFonts)
            {
                if (font == configFont)
                {
                    selectedFont = c;
                    break;
                }

                c++;
            }
        }

        private Color DrawColorControl(string name, Color value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name);
            GUILayout.FlexibleSpace();
            var newColor = GUIControls.CustomValueField(name, string.Empty, GUIControls.PresentColor, value);
            GUILayout.EndHorizontal();
            return newColor;
        }

        private void DrawWindow()
        {
            var config = ModTools.Instance.config;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Font");

            var newSelectedFont = GUIComboBox.Box(selectedFont, availableFonts, "SceneExplorerColorConfigFontsComboBox");
            if (newSelectedFont != selectedFont)
            {
                config.fontName = availableFonts[newSelectedFont];
                selectedFont = newSelectedFont;
                UpdateFont();
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            GUILayout.Label("Font size");

            var newFontSize = (int)GUILayout.HorizontalSlider(config.fontSize, 13.0f, 39.0f, GUILayout.Width(256));

            if (newFontSize != config.fontSize)
            {
                config.fontSize = newFontSize;
                UpdateFont();
            }

            GUILayout.EndHorizontal();

            var newColor = DrawColorControl("Background", config.backgroundColor);
            if (newColor != config.backgroundColor)
            {
                config.backgroundColor = newColor;
                bgTexture.SetPixel(0, 0, config.backgroundColor);
                bgTexture.Apply();
            }

            newColor = DrawColorControl("Titlebar", config.titlebarColor);
            if (newColor != config.titlebarColor)
            {
                config.titlebarColor = newColor;
                moveNormalTexture.SetPixel(0, 0, config.titlebarColor);
                moveNormalTexture.Apply();

                moveHoverTexture.SetPixel(0, 0, config.titlebarColor * 1.2f);
                moveHoverTexture.Apply();
            }

            config.titlebarTextColor = DrawColorControl("Titlebar text", config.titlebarTextColor);

            config.gameObjectColor = DrawColorControl("GameObject", config.gameObjectColor);
            config.enabledComponentColor = DrawColorControl("Component (enabled)", config.enabledComponentColor);
            config.disabledComponentColor = DrawColorControl("Component (disabled)", config.disabledComponentColor);
            config.selectedComponentColor = DrawColorControl("Selected component", config.selectedComponentColor);
            config.keywordColor = DrawColorControl("Keyword", config.keywordColor);
            config.nameColor = DrawColorControl("Member name", config.nameColor);
            config.typeColor = DrawColorControl("Member type", config.typeColor);
            config.modifierColor = DrawColorControl("Member modifier", config.modifierColor);
            config.memberTypeColor = DrawColorControl("Field type", config.memberTypeColor);
            config.valueColor = DrawColorControl("Member value", config.valueColor);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Save", GUILayout.Width(128)))
            {
                ModTools.Instance.SaveConfig();
            }

            if (GUILayout.Button("Reset", GUILayout.Width(128)))
            {
                var template = new Configuration();

                config.backgroundColor = template.backgroundColor;
                bgTexture.SetPixel(0, 0, config.backgroundColor);
                bgTexture.Apply();

                config.titlebarColor = template.titlebarColor;
                moveNormalTexture.SetPixel(0, 0, config.titlebarColor);
                moveNormalTexture.Apply();

                moveHoverTexture.SetPixel(0, 0, config.titlebarColor * 1.2f);
                moveHoverTexture.Apply();

                config.titlebarTextColor = template.titlebarTextColor;

                config.gameObjectColor = template.gameObjectColor;
                config.enabledComponentColor = template.enabledComponentColor;
                config.disabledComponentColor = template.disabledComponentColor;
                config.selectedComponentColor = template.selectedComponentColor;
                config.nameColor = template.nameColor;
                config.typeColor = template.typeColor;
                config.modifierColor = template.modifierColor;
                config.memberTypeColor = template.memberTypeColor;
                config.valueColor = template.valueColor;
                config.fontName = template.fontName;
                config.fontSize = template.fontSize;

                UpdateFont();
                ModTools.Instance.SaveConfig();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void HandleException(Exception ex)
        {
            Log.Error("Exception in SceneExplorerColorConfig - " + ex.Message);
            visible = false;
        }
    }
}