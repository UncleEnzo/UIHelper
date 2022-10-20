using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nevelson.UIHelper
{
    public class UICancelHandler : EventTrigger, ICancelHandler
    {
        UIScreenBase uiScreen;
        Popup popup;
        bool Dropdown { get => GetComponent<Dropdown>() == null; }
        bool dropdownIsOpen;

        public void Init(UIScreenBase uiScreen, Popup popup = null)
        {
            this.uiScreen = uiScreen;
            this.popup = popup;
        }

        public override void OnCancel(BaseEventData eventData)
        {
            base.OnCancel(eventData);

            if (dropdownIsOpen)
            {
                return;
            }

            if (popup != null)
            {
                popup.OnClick_ClosePopup();
                return;
            }

            uiScreen.OnClick_ScreenBack();
        }

        //If there are any other components with different selectable functionality
        //We need to make them prioritized in this as well

        public override void OnSelect(BaseEventData eventData)
        {
            if (Dropdown)
            {
                dropdownIsOpen = false;
            }
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            if (Dropdown)
            {
                dropdownIsOpen = true;
            }
        }
    }
}