using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Nevelson.UIHelper
{
    public class TabManager : MonoBehaviour, IUIManager, ISetUIFocus
    {
        [SerializeField] int startingTab;
        [SerializeField] Tab[] tabs;

        Tab currentTab;
        EventSystem unityEventSystem;
        TabButton selectedTab;
        bool isUsingController = false;

        public void UIReset()
        {
            SelectTab(tabs[startingTab].button);
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
            SelectTab(button);
            SetUIFocus();
        }

        void Start()
        {
            unityEventSystem = EventSystem.current;
            foreach (var tab in tabs)
            {
                tab.button.TabGroup = this;
            }
            UIReset();
        }

        void SelectTab(TabButton button)
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
                if (i == index)
                {
                    tabs[i].tabPage.SetActive(true);
                    currentTab = tabs[i];
                }
                else
                {
                    tabs[i].tabPage.SetActive(false);
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