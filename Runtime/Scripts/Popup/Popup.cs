using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nevelson.UIHelper
{
    public class Popup : MonoBehaviour, ISelectables, ISetUIFocus
    {
        public UnityEvent<Action, GameObject> AnimateAppearPopup;
        public UnityEvent<Action, GameObject> AnimateClosePopup;
        [SerializeField] GameObject focusTargetOnDisplay;
        Dictionary<Selectable, Navigation> selectables;
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
            Debug.Log("Popup Locking Selectables");
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
            Debug.Log("Popup Unlocking Selectables");
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

        public void AnimateOpen(Stack<Popup> openPopups)
        {
            if (AnimateAppearPopup.GetPersistentEventCount() == 0)
            {
                UnlockSelectables();
                SetUIFocus();
                openPopups.Push(this);
            }
            else if (AnimateAppearPopup.GetPersistentEventCount() == 1)
            {
                AnimateAppearPopup.Invoke(
                    () =>
                    {
                        UnlockSelectables();
                        SetUIFocus();
                        openPopups.Push(this);
                    },
                    gameObject
                );
            }
            else
            {
                Debug.LogError($"Animate popup appear does not support more than one event");
            }
        }

        public void AnimateClose(Action CleanUpPopup)
        {
            if (AnimateClosePopup.GetPersistentEventCount() == 0)
            {
                CleanUpPopup();
            }
            else if (AnimateClosePopup.GetPersistentEventCount() == 1)
            {
                AnimateClosePopup.Invoke(
                    () => CleanUpPopup(),
                    gameObject
                );
            }
            else
            {
                Debug.LogError($"Animate popup close does not support more than one event");
            }
        }

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
            Debug.Log($"Popup Regenerating selectables dictionary");

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

        public void SetUsingController(bool isUsingController)
        {
            this.isUsingController = isUsingController;
        }

        public void SetUIFocus()
        {
            if (!isUsingController)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }

            if (focusTargetOnDisplay == null)
            {
                Debug.LogError($"Attempting to set focus for popup element {gameObject.name} that has focusTargetOnDisplay = null");
                return;
            }

            EventSystem.current.SetSelectedGameObject(focusTargetOnDisplay);
        }

        void Awake()
        {
            navigationNone.mode = Navigation.Mode.None;
        }
    }
}