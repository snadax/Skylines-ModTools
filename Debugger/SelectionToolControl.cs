using System;
using ColossalFramework;
using ColossalFramework.UI;
using ModTools.Utils;
using UnityEngine;

namespace ModTools
{
    public class SelectionToolControl : MonoBehaviour
    {
        private UIButton m_Button;
        private UITiledSprite m_Bar;
        private UIComponent m_FullscreenContainer;

        public void Awake()
        {
            var toolController = FindObjectOfType<ToolManager>().m_properties;
            if (toolController == null)
            {
                return;
            }

            toolController.AddTool<SelectionTool>();
            var textureButton = AtlasUtil.LoadTextureFromAssembly("ModTools.SelectionToolButton.png");
            textureButton.name = "SelectionToolButton";
            var textureBar = AtlasUtil.LoadTextureFromAssembly("ModTools.SelectionToolBar.png");
            textureBar.name = "SelectionToolBar";

            var mode = ToolManager.instance.m_properties.m_mode;
            var baseSpriteName =
                mode == ItemClass.Availability.AssetEditor
                    ? "InfoIconBase"
                    : mode == ItemClass.Availability.Game
                        ? "RoundBackBig"
                        : "OptionBase";
            var atlas = AtlasUtil.CreateAtlas(new[]
            {
                textureButton,
                textureBar,
                GetTextureByName($"{baseSpriteName}Disabled"),
                GetTextureByName($"{baseSpriteName}Hovered"),
                GetTextureByName($"{baseSpriteName}Pressed"),
                GetTextureByName(
                    mode == ItemClass.Availability.AssetEditor ? $"{baseSpriteName}Normal" : baseSpriteName),
            });
            var buttonGo = new GameObject("SelectionToolButton");
            m_Button = buttonGo.AddComponent<UIButton>();
            m_Button.tooltip = "Mod Tools - Selection Tool";
            m_Button.normalFgSprite = "SelectionToolButton";
            m_Button.hoveredFgSprite = "SelectionToolButton";
            m_Button.pressedFgSprite = "SelectionToolButton";
            m_Button.disabledFgSprite = "SelectionToolButton";
            m_Button.focusedFgSprite = "SelectionToolButton";
            m_Button.normalBgSprite =
                mode == ItemClass.Availability.AssetEditor ? $"{baseSpriteName}Normal" : baseSpriteName;
            m_Button.focusedBgSprite =
                mode == ItemClass.Availability.AssetEditor ? $"{baseSpriteName}Normal" : baseSpriteName;
            m_Button.hoveredBgSprite = $"{baseSpriteName}Hovered";
            m_Button.pressedBgSprite = $"{baseSpriteName}Pressed";
            m_Button.disabledBgSprite = $"{baseSpriteName}Disabled";
            m_Button.absolutePosition = new Vector3(1700, 10); // TODO: make dynamic
            m_Button.playAudioEvents = true;
            m_Button.width = 46f;
            m_Button.height = 46f;
            UIView.GetAView().AttachUIComponent(buttonGo);
            m_Button.atlas = atlas;
            m_Button.eventClicked += (c, e) => ToggleTool();

            var dragGo = new GameObject("SelectionToolDragHandler");
            dragGo.transform.parent = transform;
            dragGo.transform.localPosition = Vector3.zero;
            var drag = dragGo.AddComponent<UIDragHandle>();
            drag.tooltip = m_Button.tooltip;
            drag.width = m_Button.width;
            drag.height = m_Button.height;
            
            var barGo = new GameObject("SelectionToolBar");
            m_Bar = barGo.AddComponent<UITiledSprite>();
            UIView.GetAView().AttachUIComponent(barGo);
            m_Bar.atlas = atlas;
            m_Bar.width = UIView.GetAView().fixedWidth;
            var relativePosition = m_Bar.relativePosition;
            relativePosition.x = 0;
            m_Bar.relativePosition = relativePosition;
            m_Bar.height = 28;
            m_Bar.zOrder = 18;
            m_Bar.spriteName = "SelectionToolBar";
            m_Bar.Hide();

            this.m_FullscreenContainer = UIView.Find("FullScreenContainer");
        }

        public void Update()
        {
            var tool = ToolsModifierControl.GetTool<SelectionTool>();
            if (tool == null)
            {
                return;
            }

            if (!tool.enabled && m_Bar.isVisible)
            {
                m_Bar.Hide();
            }

            if (!MainWindow.Instance.Config.SelectionTool)
            {
                return;
            }

            if ((!Input.GetKey(KeyCode.RightControl) && !Input.GetKey(KeyCode.LeftControl)) ||
                !Input.GetKeyDown(KeyCode.M))
            {
                return;
            }

            ToggleTool();
        }

        private void ToggleTool()
        {
            var tool = ToolsModifierControl.GetTool<SelectionTool>();
            if (tool == null)
            {
                return;
            }

            if (tool.enabled)
            {
                ValueAnimator.Animate("BulldozerBar", (Action<float>) (val =>
                    {
                        Vector3 relativePosition = this.m_Bar.relativePosition;
                        relativePosition.y = val;
                        this.m_Bar.relativePosition = relativePosition;
                    }),
                    new AnimatedFloat(
                        this.m_FullscreenContainer.relativePosition.y + this.m_FullscreenContainer.size.y -
                        this.m_Bar.size.y,
                        this.m_FullscreenContainer.relativePosition.y + this.m_FullscreenContainer.size.y, 0.3f),
                    (Action) (() => this.m_Bar.Hide()));

                ToolsModifierControl.SetTool<DefaultTool>();
            }
            else
            {
                ToolsModifierControl.mainToolbar.CloseEverything();
                ToolsModifierControl.SetTool<SelectionTool>();
                this.m_Bar.Show();
                ValueAnimator.Animate("BulldozerBar", (Action<float>) (val =>
                    {
                        Vector3 relativePosition = this.m_Bar.relativePosition;
                        relativePosition.y = val;
                        this.m_Bar.relativePosition = relativePosition;
                    }),
                    new AnimatedFloat(this.m_FullscreenContainer.relativePosition.y + this.m_FullscreenContainer.size.y,
                        this.m_FullscreenContainer.relativePosition.y + this.m_FullscreenContainer.size.y -
                        this.m_Bar.size.y, 0.3f));
            }
        }

        private static Texture2D GetTextureByName(string name)
        {
            return UIView.GetAView().defaultAtlas.sprites.Find(sprite => sprite.name == name).texture;
        }
    }
}