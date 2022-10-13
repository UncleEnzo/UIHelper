using System;
using UnityEngine;

namespace Nevelson.UIHelper
{
    public class TabGroup : MonoBehaviour
    {
        [SerializeField] int startingTab;
        //Note: Can also make a color changing version of this, just add that option
        [SerializeField] Sprite tabIdle;
        [SerializeField] Sprite tabHover;
        [SerializeField] Sprite tabActive;
        [SerializeField] Tab[] tabs;

        TabButton selectedTab;

        public void OnTabEnter(TabButton button)
        {
            ResetTabs();
            if (selectedTab != null && button == selectedTab)
            {
                return;
            }
            button.Background.sprite = tabHover;
        }

        public void OnTabExit(TabButton button)
        {
            ResetTabs();
        }

        public void OnTabSelected(TabButton button)
        {
            if (selectedTab != null)
            {
                selectedTab.Deselect();
            }

            selectedTab = button;
            selectedTab.Select();
            ResetTabs();
            button.Background.sprite = tabActive;
            int index = button.transform.GetSiblingIndex();
            for (int i = 0; i < tabs.Length; i++)
            {
                if (i == index)
                {
                    tabs[i].tabPage.SetActive(true);
                }
                else
                {
                    tabs[i].tabPage.SetActive(false);
                }
            }
        }

        public void ResetTabs()
        {
            foreach (Tab tab in tabs)
            {
                if (selectedTab != null && tab.button == selectedTab)
                {
                    continue;
                }
                tab.button.Background.sprite = tabIdle;
            }
        }

        void Start()
        {
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
    }
}