using ColossalFramework.UI;
using UnityEngine;

namespace ModTools
{
    class SelectionToolButton : UIButton
    {
        public override void Start()
        {
            
            absolutePosition = new Vector3(1700, 10);
            playAudioEvents = true;
            tooltip = "Mod Tools - Selection Tool";
            normalFgSprite = "SelectionToolButton";
            hoveredFgSprite = "SelectionToolButton";
            pressedFgSprite = "SelectionToolButton";
            disabledFgSprite = "SelectionToolButton";
            focusedFgSprite  = "SelectionToolButton";
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
    }
}