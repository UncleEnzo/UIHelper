using System;
using UnityEngine;

namespace Nevelson.UIHelper
{
    public class TabGroup : MonoBehaviour
    {
        [SerializeField] int startingTab;
        [SerializeField] Tab[] tabs;

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