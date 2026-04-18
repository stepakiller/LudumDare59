using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class ChooseCategorie : MonoBehaviour
{
    [Serializable]
    public struct TabMapping
    {
        public Button tabButton;
        public GameObject panel;
    }
    [SerializeField] List<TabMapping> tabs;
    [SerializeField] int startTabIndex = 0;

    void Start() => InitTabs();

    void InitTabs()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            int index = i;
            tabs[i].tabButton.onClick.AddListener(() => OpenTab(index));
        }
        OpenTab(startTabIndex);
    }

    public void OpenTab(int targetIndex)
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            bool isActive = (i == targetIndex);
            tabs[i].panel.SetActive(isActive);
        }
    }
}