using ColossalFramework.UI;
using ModTools.Utils;
using UnityEngine;

namespace ModTools
{
    class SelectionToolButton : UIButton
    {

        private bool toggleState;

        public bool ToggleState
        {
            get { return toggleState; }
            set {

                if (toggleState != value)
                {
                    normalBgSprite = !toggleState ? "BgActive" : "BgNormal";
                    hoveredBgSprite = !toggleState ? "BgActive" : "BgHover";
                    toggleState = value;
                }
            }
        }

        public static UIButton Create()
        {

            var go = new GameObject("SelectionToolButton");
            var toolButton =  go.AddComponent<SelectionToolButton>();

            UIView.GetAView().AttachUIComponent(go);

            return toolButton;
        }


        public override void Start()
        {
            
            absolutePosition = new Vector3(1700, 10);

            var texture = AtlasUtil.LoadTextureFromAssembly("ModTools.SelectionToolNormal.png");
            texture.name = "SelectionToolNormal";
            atlas = AtlasUtil.CreateAtlas(new[]
            {
                texture,
                GetTextureByName("RoundBackBigDisabled"),
                GetTextureByName("RoundBackBigHovered"),
                GetTextureByName("RoundBackBigPressed"),
                GetTextureByName("RoundBackBig"),
            });

            playAudioEvents = true;
            tooltip = "Mod Tools - Selection Tool";
            normalFgSprite = texture.name;
            hoveredFgSprite = texture.name;
            pressedFgSprite = texture.name;
            disabledFgSprite = texture.name;
            focusedFgSprite  = texture.name;
            normalBgSprite = "RoundBackBig";
            focusedBgSprite = "RoundBackBig";
            hoveredBgSprite = "RoundBackBigHovered";
            pressedBgSprite = "RoundBackBigPressed";
            disabledBgSprite = "RoundBackBigDisabled";
            width = 46f;
            height = 46f;

            var dragGo = new GameObject("DragHandler");
            dragGo.transform.parent = transform;
            dragGo.transform.localPosition = Vector3.zero;
            var drag = dragGo.AddComponent<UIDragHandle>();
            drag.tooltip = tooltip;
            drag.width = width;
            drag.height = height;

            base.Start();
        }

        private static Texture2D GetTextureByName(string name)
        {
            return UIView.GetAView().defaultAtlas.sprites.Find(sprite => sprite.name == name).texture;
        }

    }
}