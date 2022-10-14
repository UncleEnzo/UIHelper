using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Nevelson.UIHelper
{
    public class TabGroup : MonoBehaviour
    {
        [SerializeField] int startingTab;
        [SerializeField] Tab[] tabs;

        EventSystem unityEventSystem;
        TabButton selectedTab;

        public void OnTabEnter(TabButton button)
        {
            //No-Op as of yet, may expose or something later
        }

        public void OnTabExit(TabButton button)
        {
            //No-Op as of yet, may expose or something later
        }

        public void OnTabSelected(TabButton button)
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
                    if (tabs[i].setFocusToGameObject != null)
                    {
                        unityEventSystem.SetSelectedGameObject(tabs[i].setFocusToGameObject);
                    }
                }
                else
                {
                    tabs[i].tabPage.SetActive(false);
                }
            }
        }

        void Start()
        {
            unityEventSystem = EventSystem.current;
            foreach (var tab in tabs)
            {
                tab.button.TabGroup = this;
            }
            OnTabSelected(tabs[startingTab].button);
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