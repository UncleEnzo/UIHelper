using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nevelson.UIHelper
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIScreenBase : ButtonsBase, IScreen
    {
        [SerializeField] GameObject focusTargetOnDisplay;
        EventSystem unityEventSystem;
        CanvasGroup _canvasGroup;
        Dictionary<Selectable, Navigation> selectables;
        Navigation navigationNone = new Navigation();
        bool isScreenDisplayed;

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

        public void BeforeDisplay()
        {
            UnlockScreenSelectables();
        }

        public virtual void Display()
        {
            CanvasGroup.alpha = 1f;
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
        }

        public void AfterDisplay()
        {
            SetUIFocus();
            isScreenDisplayed = true;
        }

        public void BeforeHide()
        {
            LockScreenSelectables();
        }

        public virtual void Hide()
        {
            CanvasGroup.alpha = 0f;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
        }

        public void AfterHide()
        {
            isScreenDisplayed = false;
        }

        protected virtual void Awake()
        {
            navigationNone.mode = Navigation.Mode.None;
        }

        protected virtual void Start()
        {
            unityEventSystem = EventSystem.current;
        }

        protected virtual void Update() { }

        void LockScreenSelectables()
        {
            InitSelectables();
            foreach (KeyValuePair<Selectable, Navigation> selectable in selectables)
            {
                Debug.Log($"Locking UI element: {selectable.Key.gameObject.name}");

                selectable.Key.interactable = false;
                selectable.Key.navigation = navigationNone;
            }
        }

        void UnlockScreenSelectables()
        {
            InitSelectables();
            foreach (KeyValuePair<Selectable, Navigation> selectable in selectables)
            {
                Debug.Log($"Unlocking UI element: {selectable.Key.gameObject.name}");
                selectable.Key.interactable = true;
                selectable.Key.navigation = selectable.Value;
            }
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

        void SetUIFocus()
        {
            if (focusTargetOnDisplay == null)
            {
                return;
            }

            unityEventSystem.SetSelectedGameObject(focusTargetOnDisplay);
        }
    }
}