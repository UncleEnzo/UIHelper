using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nevelson.UIHelper
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIScreenBase : ButtonsBase, IScreen, ISelectables, ISetUIFocus
    {
        [SerializeField] GameObject focusTargetOnDisplay;
        [SerializeField] UnityEvent<Action> animateScreenDisplay;
        [SerializeField] UnityEvent onDisplay;
        [SerializeField] UnityEvent<Action> animateScreenHide;
        [SerializeField] UnityEvent onHide;

        EventSystem unityEventSystem;
        CanvasGroup _canvasGroup;
        Dictionary<Selectable, Navigation> selectables;
        Navigation navigationNone = new Navigation();
        PopupManager[] iPopupManagers;
        TabManager[] iTabManagers;
        bool isUsingController = false;
        bool isScreenDisplayed = true;

        CanvasGroup CanvasGroup
        {
            get
            {
                if (_canvasGroup == null)
                {
                    _canvasGroup = GetComponent<CanvasGroup>();
                }
                return _canvasGroup;
            }
        }

        public virtual void Display()
        {
            if (isScreenDisplayed)
            {
                Debug.LogError($"Attempting to display screen that is already displayed {gameObject.name}");
                return;
            }
            Debug.Log($"Calling Display on {gameObject.name}");
            onDisplay?.Invoke();
            CanvasGroup.alpha = 1f;
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
            if (animateScreenDisplay.GetPersistentEventCount() == 0)
            {
                UnlockSelectables();
                SetUIFocus();
                isScreenDisplayed = true;
            }
            else if (animateScreenDisplay.GetPersistentEventCount() == 1)
            {
                animateScreenDisplay.Invoke(() =>
                {
                    Debug.Log("Unlocking Screen");
                    UnlockSelectables();
                    SetUIFocus();
                    isScreenDisplayed = true;
                });
            }
            else
            {
                Debug.LogError($"Animate screen display does not support more than one ");
            }
        }

        public virtual void Hide()
        {
            if (!isScreenDisplayed)
            {
                return;
            }

            onHide?.Invoke();
            if (iPopupManagers != null && iPopupManagers.Length != 0)
            {
                foreach (var manager in iPopupManagers)
                {
                    //Debug.Log($"{this.gameObject.name} calling reset managers");
                    manager.UIReset();
                }
            }

            if (iTabManagers != null && iTabManagers.Length != 0)
            {
                foreach (var manager in iTabManagers)
                {
                    //Debug.Log($"{this.gameObject.name} calling reset managers");
                    manager.UIReset();
                }
            }

            LockSelectables();

            if (animateScreenHide.GetPersistentEventCount() == 0)
            {
                CanvasGroup.alpha = 0f;
                CanvasGroup.interactable = false;
                CanvasGroup.blocksRaycasts = false;
                isScreenDisplayed = false;
            }
            else if (animateScreenHide.GetPersistentEventCount() == 1)
            {
                animateScreenHide.Invoke(() =>
                {
                    CanvasGroup.alpha = 0f;
                    CanvasGroup.interactable = false;
                    CanvasGroup.blocksRaycasts = false;
                    isScreenDisplayed = false;
                });
            }
            else
            {
                Debug.LogError($"Animate screen hide does not support more than one ");
            }
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
            //Debug.Log($"{gameObject.name} is calling SetUIFocus. Is using controller for screen set to {isUsingController}");
            foreach (var manager in iPopupManagers)
            {
                manager.SetUsingController(isUsingController);
            }

            foreach (var manager in iTabManagers)
            {
                manager.SetUsingController(isUsingController);
            }

            if (!isUsingController)
            {
                unityEventSystem.SetSelectedGameObject(null);
                return;
            }

            if (focusTargetOnDisplay == null && isUsingController)
            {
                Debug.LogError($"Attempting to use controller but {gameObject.name} focusTargetOnDisplay field is null.");
                return;
            }

            unityEventSystem.SetSelectedGameObject(focusTargetOnDisplay);

            //if there is an active tab, sets the focus to that
            foreach (var manager in iTabManagers)
            {
                manager.SetUIFocus();
            }

            //if there is an active tab, if there is an active popup, sets the focus to that
            foreach (var manager in iPopupManagers)
            {
                manager.SetUIFocus();
            }
        }

        protected virtual void Awake()
        {
            navigationNone.mode = Navigation.Mode.None;
            iPopupManagers = GetComponentsInChildren<PopupManager>();
            iTabManagers = GetComponentsInChildren<TabManager>();
            if (iPopupManagers.Length > 1)
            {
                Debug.LogError($"Screen {gameObject.name} has more than 1 popupmanager.  if you need multiple popups, use the same manager");
            }
            if (iTabManagers.Length > 1)
            {
                Debug.LogError($"Screen {gameObject.name} has more than 1 popupmanager.  if you need multiple tabs, use the same manager");
            }
        }

        protected virtual void Start()
        {
            unityEventSystem = EventSystem.current;
        }

        protected virtual void Update() { }

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
    }
}