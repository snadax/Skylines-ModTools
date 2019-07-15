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
            var atlas = AtlasUtil.CreateAtlas(new[]
            {
                textureButton,
                textureBar,
                GetTextureByName("RoundBackBigDisabled"),
                GetTextureByName("RoundBackBigHovered"),
                GetTextureByName("RoundBackBigPressed"),
                GetTextureByName("RoundBackBig"),
            });
            var buttonGo = new GameObject("SelectionToolButton");
            m_Button = buttonGo.AddComponent<SelectionToolButton>();
            UIView.GetAView().AttachUIComponent(buttonGo);
            m_Button.atlas = atlas;
            m_Button.eventClicked += (c, e) => ToggleTool();
            
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
                ValueAnimator.Animate("BulldozerBar", (System.Action<float>) (val =>
                {
                    Vector3 relativePosition = this.m_Bar.relativePosition;
                    relativePosition.y = val;
                    this.m_Bar.relativePosition = relativePosition;
                }), new AnimatedFloat(this.m_FullscreenContainer.relativePosition.y + this.m_FullscreenContainer.size.y - this.m_Bar.size.y, this.m_FullscreenContainer.relativePosition.y + this.m_FullscreenContainer.size.y, 0.3f), (System.Action) (() => this.m_Bar.Hide()));

                ToolsModifierControl.SetTool<DefaultTool>();
            }
            else
            {
                ToolsModifierControl.mainToolbar.CloseEverything();
                ToolsModifierControl.SetTool<SelectionTool>();
                this.m_Bar.Show();
                ValueAnimator.Animate("BulldozerBar", (System.Action<float>) (val =>
                {
                    Vector3 relativePosition = this.m_Bar.relativePosition;
                    relativePosition.y = val;
                    this.m_Bar.relativePosition = relativePosition;
                }), new AnimatedFloat(this.m_FullscreenContainer.relativePosition.y + this.m_FullscreenContainer.size.y, this.m_FullscreenContainer.relativePosition.y + this.m_FullscreenContainer.size.y - this.m_Bar.size.y, 0.3f));
            }
        }
        
        private static Texture2D GetTextureByName(string name)
        {
            return UIView.GetAView().defaultAtlas.sprites.Find(sprite => sprite.name == name).texture;
        }
    }
}