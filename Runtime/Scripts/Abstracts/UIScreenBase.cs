using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nevelson.UIHelper
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIScreenBase : ButtonsBase, IScreen
    {
        Dictionary<Selectable, Navigation> selectables;
        Navigation navigationNone = new Navigation();
        CanvasGroup _canvasGroup;
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
            //todo need to enable ui elements that player can focus on for controller
            //todo make it so that it focuses if using controller
        }

        public void AfterDisplay()
        {
            SetUIFocus();
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
            //TODO: NEED TO SET PLAYER FOCUS ON APPEARING | IF USING CONTROLLER
            throw new System.NotImplementedException();
        }

        protected virtual void Awake()
        {
            navigationNone.mode = Navigation.Mode.None;
        }
        protected virtual void Start() { }
        protected virtual void Update() { }

        void LockScreenSelectables()
        {
            if (selectables == null)
            {
                selectables = new Dictionary<Selectable, Navigation>();
                Selectable[] selectableUIElements = GetComponentsInChildren<Selectable>();
                foreach (var selectable in selectableUIElements)
                {
                    selectables.Add(selectable, selectable.navigation);
                }
            }

            foreach (KeyValuePair<Selectable, Navigation> selectable in selectables)
            {
                Debug.Log($"Locking UI element: {selectable.Key.gameObject.name}");

                selectable.Key.interactable = false;
                selectable.Key.navigation = navigationNone;
            }

        }

        void UnlockScreenSelectables()
        {

        }

        void SetUIFocus()
        {
            throw new System.NotImplementedException();
        }
    }
}