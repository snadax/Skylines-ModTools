using System;
using System.Collections.Generic;
using ColossalFramework.UI;

namespace ModTools.GamePanels
{
    internal sealed class ButtonsInfoPanelExtension<T> : InfoPanelExtensionBase<T>
        where T : WorldInfoPanel
    {
        private readonly List<UIButton> createdButtons;
        private readonly IDictionary<string, Action<InstanceID>> additionalButtons;

        private ButtonsInfoPanelExtension(
            string infoPanelName,
            Func<InstanceID, string> assetNameGetter,
            Action<InstanceID> displayAction,
            IDictionary<string, Action<InstanceID>> buttons)
            : base(infoPanelName, assetNameGetter, displayAction)
        {
            additionalButtons = new Dictionary<string, Action<InstanceID>>(buttons);
            createdButtons = new List<UIButton>(additionalButtons.Count);
        }

        public static ButtonsInfoPanelExtension<T> Create(
            string panelName,
            Func<InstanceID, string> assetNameGetter,
            Action<InstanceID> displayAction,
            IDictionary<string, Action<InstanceID>> buttons)
        {
            if (string.IsNullOrEmpty(panelName))
            {
                throw new ArgumentException("The panel name cannot be null or empty string.", nameof(panelName));
            }

            if (assetNameGetter == null)
            {
                throw new ArgumentNullException(nameof(assetNameGetter));
            }

            if (displayAction == null)
            {
                throw new ArgumentNullException(nameof(displayAction));
            }

            if (buttons == null)
            {
                throw new ArgumentNullException(nameof(buttons));
            }

            var result = new ButtonsInfoPanelExtension<T>(panelName, assetNameGetter, displayAction, buttons);
            return result.Initialize() ? result : null;
        }

        protected override void DisableCore()
        {
            foreach (var button in createdButtons)
            {
                button.eventClick -= AdditionalButtonClick;
                ItemsPanel.RemoveUIComponent(button);
                UnityEngine.Object.Destroy(button);
            }

            createdButtons.Clear();
            additionalButtons.Clear();
        }

        protected override bool InitializeCore()
        {
            foreach (var buttonDefinition in additionalButtons)
            {
                var newButton = AddButton(buttonDefinition.Key);
                if (newButton == null)
                {
                    return false;
                }

                newButton.eventClick += AdditionalButtonClick;
                createdButtons.Add(newButton);
            }

            return true;
        }

        private void AdditionalButtonClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            var buttonCaption = ((UIButton)component).text;
            var action = additionalButtons[buttonCaption];
            action(CurrentInstanceId);
        }
    }
}
