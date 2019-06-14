using ColossalFramework.UI;

namespace ModTools
{
    internal sealed class ModalUI
    {
        private UIComponent modalView;
        private bool isModal;

        public void Update(bool mouseOverWindow)
        {
            if (modalView == null)
            {
                modalView = UIView.GetAView()?.AddUIComponent(typeof(UILabel));
            }

            if (modalView == null)
            {
                return;
            }

            if (!mouseOverWindow)
            {
                if (UIView.ModalInputCount() > 0 && UIView.GetModalComponent() == modalView)
                {
                    isModal = false;
                    UIView.PopModal();
                }
            }
            else if (!isModal)
            {
                isModal = true;
                UIView.PushModal(modalView);
            }
        }
    }
}
