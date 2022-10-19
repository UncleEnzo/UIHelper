using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nevelson.UIHelper
{
    public class UICancelHandler : EventTrigger, ICancelHandler
    {
        UIScreenBase uiScreen;
        bool Dropdown { get => GetComponent<Dropdown>() == null; }
        bool dropdownIsOpen;

        public void Init(UIScreenBase uiScreen)
        {
            this.uiScreen = uiScreen;
        }

        public override void OnCancel(BaseEventData eventData)
        {
            base.OnCancel(eventData);

            if (!dropdownIsOpen)
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