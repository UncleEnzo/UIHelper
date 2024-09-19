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

        [SerializeField] UnityEvent<Action, GameObject> animateAppearPopup;
        [SerializeField] UnityEvent<Action, GameObject> animateClosePopup;
        [SerializeField] GameObject focusTargetOnDisplay;
        Dictionary<Selectable, Navigation> selectables;
        Action<Popup> closePopup;
        Navigation navigationNone = new Navigation();
        bool isUsingController;
        int animateAppearSubscribeCounter = 0;
        int animateCloseSubscribeCounter = 0;
        int callsAppear = 0;
        int callsClose = 0;
        int callsRequiredAppear;

        public void SubscribeAppearPopup(UnityAction<Action, GameObject> action)
        {
            animateAppearSubscribeCounter++;
            animateAppearPopup.AddListener(action);
        }

        public void SubscribeClosePopup(UnityAction<Action, GameObject> action)
        {
            animateCloseSubscribeCounter++;
            animateClosePopup.AddListener(action);
        }

        public void RemoveAllListeners()
        {
            animateAppearSubscribeCounter = 0;
            animateCloseSubscribeCounter = 0;
            animateAppearPopup.RemoveAllListeners();
            animateClosePopup.RemoveAllListeners();
        }

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
                    Debug.LogError("HIT ERROR WITH SELECTABLES");
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
                Debug.Log($"Unlocking UI element: {selectable.Key.gameObject.name}");
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
            void DisplayCallback()
            {
                UnlockSelectables();
                SetUIFocus();
                openPopups.Push(this);
            }
            if (animateAppearPopup.GetPersistentEventCount() + animateAppearSubscribeCounter == 0)
            {
                DisplayCallback();
            }
            else if (animateAppearPopup.GetPersistentEventCount() + animateAppearSubscribeCounter == 1)
            {
                animateAppearPopup.Invoke(DisplayCallback, gameObject);
            }
            else
            {
                callsRequiredAppear = animateAppearPopup.GetPersistentEventCount() + animateAppearSubscribeCounter;

                void MultiCastDisplayCallback()
                {
                    callsAppear++;
                    if (callsAppear < callsRequiredAppear)
                    {
                        return;
                    }
                    callsAppear = 0;
                    DisplayCallback();
                }

                animateAppearPopup.Invoke(MultiCastDisplayCallback, gameObject);
            }
        }

        public void AnimateClose(Action CleanUpPopup)
        {
            if (animateClosePopup.GetPersistentEventCount() + animateCloseSubscribeCounter == 0)
            {
                CleanUpPopup();
            }
            else if (animateClosePopup.GetPersistentEventCount() + animateCloseSubscribeCounter == 1)
            {
                animateClosePopup.Invoke(CleanUpPopup, gameObject);
            }
            else
            {
                int callsRequired = animateClosePopup.GetPersistentEventCount() + animateCloseSubscribeCounter;
                void MultiCastCleanUpPopup()
                {
                    callsClose++;
                    if (callsClose < callsRequired)
                    {
                        return;
                    }
                    callsClose = 0;
                    CleanUpPopup();
                }
                animateClosePopup.Invoke(MultiCastCleanUpPopup, gameObject);
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
                Debug.Log($"ADDING SELECTABLE {gameObject.name}");
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
            Debug.Log($"REACHED THIS 0-2: Setting is using to: {isUsingController}");
            this.isUsingController = isUsingController;
        }

        public void SetUIFocus()
        {
            Debug.Log($"REACHED THIS CONTROLLER| {isUsingController}");
            if (!isUsingController)
            {
                Debug.Log("REACHED THIS, STUCK HERE FOR SOME REASON?");
                EventSystem.current.SetSelectedGameObject(null);
                return;
            }

            Debug.Log("REACHED THIS 1");
            if (focusTargetOnDisplay == null)
            {
                Debug.LogError($"Attempting to set focus for popup element {gameObject.name} that has focusTargetOnDisplay = null");
                return;
            }

            Debug.Log("REACHED THIS 2");
            EventSystem.current.SetSelectedGameObject(focusTargetOnDisplay);
        }

        void Awake()
        {
            navigationNone.mode = Navigation.Mode.None;
        }
    }
}