using System;
using UnityEngine;

namespace ModTools
{
    internal sealed class SceneExplorerColorConfig : GUIWindow
    {
        private readonly string[] availableFonts;
        private int selectedFont;

        public SceneExplorerColorConfig()
            : base("Font/ color configuration", new Rect(16.0f, 16.0f, 600.0f, 490.0f), Skin)
        {
            Visible = false;
            Resizable = false;

            availableFonts = Font.GetOSInstalledFontNames();
            var c = 0;
            var configFont = Config.FontName;

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

        private static ModConfiguration Config => ModTools.Instance.Config;

        protected override void DrawWindow()
        {
            var config = ModTools.Instance.Config;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Font");

            var newSelectedFont = GUIComboBox.Box(selectedFont, availableFonts, "SceneExplorerColorConfigFontsComboBox");
            if (newSelectedFont != selectedFont)
            {
                config.FontName = availableFonts[newSelectedFont];
                selectedFont = newSelectedFont;
                UpdateFont();
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            GUILayout.Label("Font size");

            var newFontSize = (int)GUILayout.HorizontalSlider(config.FontSize, 13.0f, 39.0f, GUILayout.Width(256));

            if (newFontSize != config.FontSize)
            {
                config.FontSize = newFontSize;
                UpdateFont();
            }

            GUILayout.EndHorizontal();

            var newColor = DrawColorControl("Background", config.BackgroundColor);
            if (newColor != config.BackgroundColor)
            {
                config.BackgroundColor = newColor;
                BgTexture.SetPixel(0, 0, config.BackgroundColor);
                BgTexture.Apply();
            }

            newColor = DrawColorControl("Titlebar", config.TitlebarColor);
            if (newColor != config.TitlebarColor)
            {
                config.TitlebarColor = newColor;
                MoveNormalTexture.SetPixel(0, 0, config.TitlebarColor);
                MoveNormalTexture.Apply();

                MoveHoverTexture.SetPixel(0, 0, config.TitlebarColor * 1.2f);
                MoveHoverTexture.Apply();
            }

            config.TitlebarTextColor = DrawColorControl("Titlebar text", config.TitlebarTextColor);

            config.GameObjectColor = DrawColorControl("GameObject", config.GameObjectColor);
            config.EnabledComponentColor = DrawColorControl("Component (enabled)", config.EnabledComponentColor);
            config.DisabledComponentColor = DrawColorControl("Component (disabled)", config.DisabledComponentColor);
            config.SelectedComponentColor = DrawColorControl("Selected component", config.SelectedComponentColor);
            config.KeywordColor = DrawColorControl("Keyword", config.KeywordColor);
            config.NameColor = DrawColorControl("Member name", config.NameColor);
            config.TypeColor = DrawColorControl("Member type", config.TypeColor);
            config.ModifierColor = DrawColorControl("Member modifier", config.ModifierColor);
            config.MemberTypeColor = DrawColorControl("Field type", config.MemberTypeColor);
            config.ValueColor = DrawColorControl("Member value", config.ValueColor);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Save", GUILayout.Width(128)))
            {
                ModTools.Instance.SaveConfig();
            }

            if (GUILayout.Button("Reset", GUILayout.Width(128)))
            {
                var template = new ModConfiguration();

                config.BackgroundColor = template.BackgroundColor;
                BgTexture.SetPixel(0, 0, config.BackgroundColor);
                BgTexture.Apply();

                config.TitlebarColor = template.TitlebarColor;
                MoveNormalTexture.SetPixel(0, 0, config.TitlebarColor);
                MoveNormalTexture.Apply();

                MoveHoverTexture.SetPixel(0, 0, config.TitlebarColor * 1.2f);
                MoveHoverTexture.Apply();

                config.TitlebarTextColor = template.TitlebarTextColor;

                config.GameObjectColor = template.GameObjectColor;
                config.EnabledComponentColor = template.EnabledComponentColor;
                config.DisabledComponentColor = template.DisabledComponentColor;
                config.SelectedComponentColor = template.SelectedComponentColor;
                config.NameColor = template.NameColor;
                config.TypeColor = template.TypeColor;
                config.ModifierColor = template.ModifierColor;
                config.MemberTypeColor = template.MemberTypeColor;
                config.ValueColor = template.ValueColor;
                config.FontName = template.FontName;
                config.FontSize = template.FontSize;

                UpdateFont();
                ModTools.Instance.SaveConfig();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        protected override void HandleException(Exception ex)
        {
            Log.Error("Exception in SceneExplorerColorConfig - " + ex.Message);
            Visible = false;
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
    }
}