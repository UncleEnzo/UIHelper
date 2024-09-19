using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nevelson.UIHelper
{
    public class UICancelHandler : EventTrigger, ICancelHandler
    {
        UIScreenBase uiScreen;
        Popup popup;
        TabManager tabManager;
        bool Dropdown { get => GetComponent<Dropdown>() == null; }
        bool dropdownIsOpen;

        public void Init(UIScreenBase uiScreen, Popup popup, TabManager tabManager)
        {
            this.uiScreen = uiScreen;
            this.popup = popup;
            this.tabManager = tabManager;
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

            if (tabManager != null)
            {
                if (tabManager.UICancel())
                {
                    return;
                }
            }

            if (uiScreen == null)
            {
                Debug.LogWarning("Could not find UI screen for cancel handler. This is not necessaryily a bug, but most likely is.  Check if there is a separate event type on the UI canceler to be sure. (if not then it's a bug)");
            }
            else
            {
                uiScreen.OnClick_ScreenBack();
            }
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