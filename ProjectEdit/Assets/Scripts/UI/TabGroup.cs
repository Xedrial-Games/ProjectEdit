using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectEdit.UI
{
    public class TabGroup : MonoBehaviour
    {
        [SerializeField] private List<GameObject> m_ObjectsToSwap;
        
        [SerializeField] private Color m_TabIdle;
        [SerializeField] private Color m_TabHovered;
        [SerializeField] private Color m_TabSelected;

        private List<TabButton> m_TabButtons;
        private TabButton m_SelectedTab;

        public void Subscribe(TabButton tabButton)
        {
            m_TabButtons ??= new List<TabButton>();
            m_TabButtons.Add(tabButton);
        }
        
        public void OnTabEnter(TabButton tabButton)
        {
            ResetTabs();

            if (!m_SelectedTab || tabButton != m_SelectedTab)
                tabButton.Background.color = m_TabHovered;
        }
        
        public void OnTabExit(TabButton tabButton)
        {
            ResetTabs();
        }
        
        public void OnTabSelected(TabButton tabButton)
        {
            if (m_SelectedTab)
                m_SelectedTab.Deselect();

            m_SelectedTab = tabButton;
            m_SelectedTab.Select();

            ResetTabs();
            tabButton.Background.color = m_TabSelected;

            int index = tabButton.transform.GetSiblingIndex();

            for (var i = 0; i < m_ObjectsToSwap.Count; i++)
                m_ObjectsToSwap[i].SetActive(i == index);
        }

        private void ResetTabs()
        {
            foreach (TabButton tabButton in m_TabButtons.Where(tabButton => !m_SelectedTab || m_SelectedTab != tabButton))
            {
                tabButton.Background.color = m_TabIdle;
            }
        }
    }
}
