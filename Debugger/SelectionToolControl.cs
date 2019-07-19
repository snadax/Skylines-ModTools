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

            var escButton = (UIButton)UIView.Find("Esc");
            var atlas = AtlasUtil.CreateAtlas(new[]
                {
                    textureButton,
                    textureBar,
                    GetTextureByName(escButton.disabledBgSprite, escButton.atlas),
                    GetTextureByName(escButton.hoveredBgSprite, escButton.atlas),
                    GetTextureByName(escButton.pressedBgSprite, escButton.atlas),
                    GetTextureByName(escButton.normalBgSprite, escButton.atlas),
                });
            var buttonGo = new GameObject("SelectionToolButton");
            m_Button = buttonGo.AddComponent<UIButton>();
            m_Button.tooltip = "Mod Tools - Selection Tool";
            m_Button.normalFgSprite = "SelectionToolButton";
            m_Button.hoveredFgSprite = "SelectionToolButton";
            m_Button.pressedFgSprite = "SelectionToolButton";
            m_Button.disabledFgSprite = "SelectionToolButton";
            m_Button.focusedFgSprite = "SelectionToolButton";
            m_Button.normalBgSprite = escButton.normalBgSprite;
            m_Button.focusedBgSprite = escButton.normalBgSprite;
            m_Button.hoveredBgSprite = escButton.hoveredBgSprite;
            m_Button.pressedBgSprite = escButton.pressedBgSprite;
            m_Button.disabledBgSprite = escButton.disabledBgSprite;
            m_Button.playAudioEvents = true;
            m_Button.width = 46f;
            m_Button.height = 46f;
            UIView.GetAView().AttachUIComponent(buttonGo);
            m_Button.absolutePosition = escButton.absolutePosition - new Vector3(95, 0, 0);
            m_Button.atlas = atlas;
            m_Button.eventClicked += (c, e) => ToggleTool();
            m_Button.isVisible = MainWindow.Instance.Config.SelectionTool;

            var dragGo = new GameObject("SelectionToolDragHandler");
            dragGo.transform.parent = m_Button.transform;
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

            if (MainWindow.Instance.Config.SelectionTool)
            {
                if (!m_Button.isVisible)
                {
                    m_Button.Show();
                }
            }
            else
            {
                if (m_Button.isVisible)
                {
                    m_Button.Hide();
                }

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
                ValueAnimator.Animate("BulldozerBar", val =>
                    {
                        Vector3 relativePosition = this.m_Bar.relativePosition;
                        relativePosition.y = val;
                        this.m_Bar.relativePosition = relativePosition;
                    },
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
                ValueAnimator.Animate("BulldozerBar", val =>
                    {
                        Vector3 relativePosition = this.m_Bar.relativePosition;
                        relativePosition.y = val;
                        this.m_Bar.relativePosition = relativePosition;
                    },
                    new AnimatedFloat(this.m_FullscreenContainer.relativePosition.y + this.m_FullscreenContainer.size.y,
                        this.m_FullscreenContainer.relativePosition.y + this.m_FullscreenContainer.size.y -
                        this.m_Bar.size.y, 0.3f));
            }
        }

        private static Texture2D GetTextureByName(string name, UITextureAtlas atlas)
        {
            return atlas.sprites.Find(sprite => sprite.name == name).texture;
        }
    }
}