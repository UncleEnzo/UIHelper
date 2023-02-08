using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] UnityEvent<Action, GameObject> animateScreenDisplay;
        [SerializeField] UnityEvent onDisplay;
        [SerializeField] UnityEvent<Action, GameObject> animateScreenHide;
        [SerializeField] UnityEvent onHide;

        CanvasGroup _canvasGroup;
        Dictionary<Selectable, Navigation> selectables;
        Navigation navigationNone = new Navigation();
        PopupManager[] iPopupManagers;
        TabManager[] iTabManagers;
        bool isUsingController = false;
        public bool IsScreenDisplayed { get; private set; } = true;

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

        public virtual void Init()
        {
            ResetUIManagers();
            LockSelectables();
            HideScreenCanvasGroup();
        }

        public virtual void Display()
        {
            if (IsScreenDisplayed)
            {
                Debug.LogError($"Attempting to display screen that is already displayed {gameObject.name}");
                return;
            }

            DisplayScreenCanvasGroup();
            if (animateScreenDisplay.GetPersistentEventCount() == 0)
            {
                UnlockSelectables();
                SetUIFocus();
                IsScreenDisplayed = true;
            }
            else if (animateScreenDisplay.GetPersistentEventCount() == 1)
            {
                isLockedForAnimation = true;
                animateScreenDisplay.Invoke(() =>
                {
                    UnlockSelectables();
                    SetUIFocus();
                    IsScreenDisplayed = true;
                    isLockedForAnimation = false;
                },
                this.gameObject);
            }
            else
            {
                Debug.LogError($"Animate screen display does not support more than one event");
            }
            onDisplay?.Invoke();
        }

        public virtual void Hide()
        {
            if (!IsScreenDisplayed)
            {
                Debug.LogError($"Attempting to hide screen that is already hidden {gameObject.name}");
                return;
            }

            onHide?.Invoke();
            ResetUIManagers();
            LockSelectables();

            if (animateScreenHide.GetPersistentEventCount() == 0)
            {
                HideScreenCanvasGroup();
            }
            else if (animateScreenHide.GetPersistentEventCount() == 1)
            {
                isLockedForAnimation = true;
                animateScreenHide.Invoke(() =>
                {
                    HideScreenCanvasGroup();
                    isLockedForAnimation = false;
                },
                gameObject
                );
            }
            else
            {
                Debug.LogError($"Animate screen hide does not support more than one event");
            }
        }

        public void LockSelectables()
        {
            Debug.Log("UIScreenBase Locking Selectables");
            InitSelectables();

            bool failure = false;
            foreach (KeyValuePair<Selectable, Navigation> selectable in selectables)
            {
                //Debug.Log($"Locking UI element: {selectable.Key.gameObject.name}");
                if (selectable.Key == null)
                {
                    failure = true;
                    break;
                }
                selectable.Key.interactable = false;
                selectable.Key.navigation = navigationNone;
            }

            if (failure)
            {
                UpdateSelectables();
                LockSelectables();
            }
        }

        public void UnlockSelectables()
        {
            Debug.Log("UIScreenBase Unlocking Selectables");
            InitSelectables();

            bool failure = false;
            foreach (KeyValuePair<Selectable, Navigation> selectable in selectables)
            {
                //Debug.Log($"Unlocking UI element: {selectable.Key.gameObject.name}");
                if (selectable.Key == null)
                {
                    failure = true;
                    break;
                }
                selectable.Key.interactable = true;
                selectable.Key.navigation = selectable.Value;
            }

            if (failure)
            {
                UpdateSelectables();
                UnlockSelectables();
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
                EventSystem.current.SetSelectedGameObject(null);
                return;
            }

            if (focusTargetOnDisplay == null && isUsingController)
            {
                Debug.LogError($"Attempting to use controller but {gameObject.name} focusTargetOnDisplay field is null.");
                return;
            }

            EventSystem.current.SetSelectedGameObject(focusTargetOnDisplay);

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
            iPopupManagers = GetComponentsInChildren<PopupManager>(true);
            iTabManagers = GetComponentsInChildren<TabManager>(true);
            if (iPopupManagers.Length > 1)
            {
                Debug.LogError($"Screen {gameObject.name} has more than 1 popupmanager.  if you need multiple popups, use the same manager");
            }
            //commenting this error out for now, doesn't seem to be an actual issue currently
            //if (iTabManagers.Length > 1)
            //{
            //    Debug.LogError($"Screen {gameObject.name} has more than 1 popupmanager.  if you need multiple tabs, use the same manager");
            //}
        }

        protected virtual void Update() { }

        void InitSelectables()
        {
            if (selectables != null)
            {
                return;
            }

            selectables = new Dictionary<Selectable, Navigation>();
            Selectable[] selectableUIElements = GetComponentsInChildren<Selectable>(true);
            foreach (var selectable in selectableUIElements)
            {
                selectables.Add(selectable, selectable.navigation);
            }
        }

        void UpdateSelectables()
        {
            Debug.Log("UIScreenBase Regenerating selectables dictionary");

            //Clever Unity specific work around to deleting nulls in unity only
            foreach (var key in selectables.Keys.ToArray())
            {
                if (key == null)
                {
                    selectables.Remove(key);
                }
            }

            Selectable[] selectableUIElements = GetComponentsInChildren<Selectable>(true);
            foreach (var selectable in selectableUIElements)
            {
                if (!selectables.ContainsKey(selectable))
                {
                    Debug.Log($"Adding new selectable: {selectable.gameObject.name}");
                    selectables.Add(selectable, selectable.navigation);
                }
            }
        }

        void ResetUIManagers()
        {
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
        }

        void DisplayScreenCanvasGroup()
        {
            CanvasGroup.alpha = 1f;
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
        }

        void HideScreenCanvasGroup()
        {
            CanvasGroup.alpha = 0f;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
            IsScreenDisplayed = false;
        }
    }
}