using System.Collections.Generic;
using UnityEngine;

namespace Nevelson.UIHelper
{
    public class PopupManager : MonoBehaviour, IUIManager, ISetUIFocus
    {
        [SerializeField] UIScreen uiScreen;
        Stack<Popup> openPopups = new Stack<Popup>();

        public void SetUsingController(bool isUsingController)
        {
            foreach (var popup in openPopups)
            {
                popup.SetUsingController(isUsingController);
            }
        }

        public void SetUIFocus()
        {
            if (openPopups.Count == 0)
            {
                Debug.Log("Not setting UI focus for popups. No popups open");
                return;
            }
            openPopups.Peek().SetUIFocus();
        }

        public void OnClick_OpenPopup(Popup popup)
        {
            if (popup == null)
            {
                Debug.LogError($"No popup specificed to open");
                return;
            }

            if (openPopups.Contains(popup))
            {
                Debug.LogError($"The popup you are trying to open {popup.gameObject.name} is currently open");
                return;
            }

            if (openPopups.Count == 0)
            {
                uiScreen.LockSelectables();
            }
            else
            {
                Popup previousPopup = openPopups.Peek();
                previousPopup.LockSelectables();
            }

            popup.gameObject.SetActive(true);

            popup.AnimateOpen(openPopups);
        }

        public void UIReset()
        {
            int maxAmount = 1000;
            int amount = 0;
            while (openPopups.Count > 0 && amount < maxAmount)
            {
                Popup popup = openPopups.Peek();
                //Debug.Log($"Resetting popup {popup.gameObject.name}");
                ResetPopup(popup);
                amount++;
                if (amount >= maxAmount)
                {
                    Debug.LogError($"Max amount of popups to close exceeded max amount: {maxAmount}");
                }
            }
        }

        void ResetPopup(Popup popup)
        {
            if (!openPopups.Contains(popup))
            {
                Debug.LogError($"The popup you are trying to close {popup.gameObject.name} is not open");
                return;
            }

            if (popup != openPopups.Peek())
            {
                Debug.LogError($"The popup you are trying to close {popup.gameObject.name} is not the active popup");
                return;
            }

            popup.gameObject.SetActive(false);
            openPopups.Pop();
        }

        void ClosePopup(Popup popup)
        {
            if (!openPopups.Contains(popup))
            {
                Debug.LogError($"The popup you are trying to close {popup.gameObject.name} is not open");
                return;
            }

            if (popup != openPopups.Peek())
            {
                Debug.LogError($"The popup you are trying to close {popup.gameObject.name} is not the active popup");
                return;
            }

            popup.LockSelectables();

            void CleanUpPopup()
            {
                popup.gameObject.SetActive(false);
                openPopups.Pop();
                if (openPopups.Count != 0)
                {
                    Popup previousPopup = openPopups.Peek();
                    previousPopup.UnlockSelectables();
                    previousPopup.SetUIFocus();
                    return;
                }
                uiScreen.UnlockSelectables();
                uiScreen.SetUIFocus();
            }

            popup.AnimateClose(CleanUpPopup);
        }

        void Awake()
        {
            foreach (var popup in GetComponentsInChildren<Popup>(true))
            {
                popup.Init(ClosePopup);
                popup.gameObject.SetActive(false);
            }
        }
    }
}