using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Nevelson.UIHelper
{
    public class TabManager : MonoBehaviour, IUIManager, ISetUIFocus
    {
        [SerializeField] UIScreenBase uiScreen;
        [SerializeField] int startingTab;
        [SerializeField] Tab[] tabs;
        [SerializeField] UnityEvent<Action, GameObject> animateAppearTab;
        [SerializeField] UnityEvent<Action, GameObject> animateDisableTab;
        Tab currentTab;
        EventSystem unityEventSystem;
        TabButton selectedTab;
        bool isUsingController = false;

        public void UIReset()
        {
            SelectTab(tabs[startingTab].button, true);
        }

        public void OnTabEnter(TabButton button)
        {
            //No-Op as of yet, may expose or something later
        }

        public void OnTabExit(TabButton button)
        {
            //No-Op as of yet, may expose or something later
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
                return;
            }

            if (isUsingController && currentTab.setFocusToGameObject == null)
            {
                Debug.LogError($"Attempting to set UI focus to tab {currentTab.tabPage.name} but tab's setFocusToGameobject = {currentTab.setFocusToGameObject}");
                return;
            }

            unityEventSystem.SetSelectedGameObject(currentTab.setFocusToGameObject);
        }

        public void OnTabSelected(TabButton button)
        {
            SelectTab(button, false);
        }

        void Awake()
        {
            unityEventSystem = EventSystem.current;
            foreach (var tab in tabs)
            {
                tab.button.TabGroup = this;
            }
            UIReset();
        }

        void SelectTab(TabButton button, bool initialize)
        {
            if (selectedTab != null)
            {
                selectedTab.Deselect();
            }

            selectedTab = button;
            selectedTab.Select();

            int index = -1;
            for (int i = 0; i < tabs.Length; i++)
            {
                if (tabs[i].button == button)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                Debug.LogError($"Could not find index of button {button.gameObject.name}");
                return;
            }

            for (int i = 0; i < tabs.Length; i++)
            {
                if (tabs[i].tabPage.activeInHierarchy)
                {
                    if (initialize)
                    {
                        tabs[i].tabPage.SetActive(false);
                        tabs[index].tabPage.SetActive(true);
                        currentTab = tabs[index];
                        SetUIFocus();
                    }
                    else
                    {
                        if (animateDisableTab.GetPersistentEventCount() == 0)
                        {
                            tabs[i].tabPage.SetActive(false);
                            tabs[index].tabPage.SetActive(true);
                            currentTab = tabs[index];
                            SetUIFocus();
                        }
                        else if (animateDisableTab.GetPersistentEventCount() == 1)
                        {
                            uiScreen.LockSelectables();
                            animateDisableTab.Invoke(
                                () =>
                                {
                                    tabs[i].tabPage.SetActive(false);
                                    tabs[index].tabPage.SetActive(true);
                                    currentTab = tabs[index];
                                    animateAppearTab.Invoke(() =>
                                    {
                                        uiScreen.UnlockSelectables();
                                        SetUIFocus();
                                    },
                                    tabs[index].tabPage);
                                },
                                tabs[i].tabPage
                            );
                        }
                        else
                        {
                            Debug.LogError($"Animate tab hide does not support more than one event");
                            Debug.LogError($"Animate appear tab does not support more than one event");
                        }
                    }
                    break;
                }
            }
        }
    }

    [Serializable]
    internal class Tab
    {
        public TabButton button;
        public GameObject tabPage;
        public GameObject setFocusToGameObject;
    }
}