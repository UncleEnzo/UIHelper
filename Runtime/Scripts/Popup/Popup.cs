using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nevelson.UIHelper
{
    public class Popup : MonoBehaviour, ISelectables, ISetUIFocus
    {
        [SerializeField] GameObject focusTargetOnDisplay;
        Dictionary<Selectable, Navigation> selectables;
        EventSystem unityEventSystem;
        Action<Popup> closePopup;
        Navigation navigationNone = new Navigation();
        bool isUsingController;

        public void Init(Action<Popup> closePopup)
        {
            this.closePopup = closePopup;
        }

        public void OnClick_ClosePopup()
        {
            closePopup(this);
        }

        public void LockSelectables()
        {
            InitSelectables();
            foreach (KeyValuePair<Selectable, Navigation> selectable in selectables)
            {
                //Debug.Log($"Locking UI element: {selectable.Key.gameObject.name}");
                selectable.Key.interactable = false;
                selectable.Key.navigation = navigationNone;
            }
        }

        public void UnlockSelectables()
        {
            InitSelectables();
            foreach (KeyValuePair<Selectable, Navigation> selectable in selectables)
            {
                //Debug.Log($"Unlocking UI element: {selectable.Key.gameObject.name}");
                selectable.Key.interactable = true;
                selectable.Key.navigation = selectable.Value;
            }
        }

        public void SetUsingController(bool isUsingController)
        {
            this.isUsingController = isUsingController;
        }

        public void SetUIFocus()
        {
            if (!isUsingController)
            {
                unityEventSystem.SetSelectedGameObject(null);
            }

            if (focusTargetOnDisplay == null)
            {
                Debug.LogError($"Attempting to set focus for popup element {gameObject.name} that has focusTargetOnDisplay = null");
                return;
            }

            unityEventSystem.SetSelectedGameObject(focusTargetOnDisplay);
        }

        void InitSelectables()
        {
            if (selectables != null)
            {
                return;
            }
            selectables = new Dictionary<Selectable, Navigation>();
            Selectable[] selectableUIElements = GetComponentsInChildren<Selectable>();
            foreach (var selectable in selectableUIElements)
            {
                selectables.Add(selectable, selectable.navigation);
            }
        }

        void Awake()
        {
            navigationNone.mode = Navigation.Mode.None;
            unityEventSystem = EventSystem.current;
        }
    }
}