using System.Collections.Generic;
using UnityEngine;

namespace Nevelson.UIHelper
{
    public class PopupManager : MonoBehaviour, IUIManager, ISetUIFocus
    {
        [SerializeField] UIScreen uiScreen;
        [SerializeField] bool allowPopupClose = true;
        Stack<Popup> openPopups = new Stack<Popup>();
        bool isUsingController;

        public void SetUsingController(bool isUsingController)
        {
            this.isUsingController = isUsingController;
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

            var popup = openPopups.Peek();
            popup.SetUIFocus();
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

            PopupEnable(popup);
            //popup.gameObject.SetActive(true);
            popup.SetUsingController(isUsingController);
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

            PopupDisable(popup);
            //popup.gameObject.SetActive(false);
            openPopups.Pop();
        }

        void ClosePopup(Popup popup)
        {
            if (!allowPopupClose) return;

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
                PopupDisable(popup);
                //popup.gameObject.SetActive(false);
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
                PopupDisable(popup);
                //popup.gameObject.SetActive(false);
            }
        }

        void PopupEnable(Popup popup)
        {
            CanvasGroup cv = popup.GetComponent<CanvasGroup>();
            cv.alpha = 1;
            cv.interactable = true;
            cv.blocksRaycasts = true;
        }

        void PopupDisable(Popup popup)
        {
            CanvasGroup cv = popup.GetComponent<CanvasGroup>();
            cv.alpha = 0;
            cv.interactable = false;
            cv.blocksRaycasts = false;
        }
    }
}