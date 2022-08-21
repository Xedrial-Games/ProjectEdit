using UnityEngine;
using TMPro;

namespace ProjectEdit.UI
{
    public class OnlineLevelCell : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_IDText;

        private Level m_Level;
        private GameManager m_GameManager;
        private GameObject m_LevelMenu;
        private GameObject m_CurrentMenu;

        private void Awake() => m_GameManager = GameManager.Instance;

        public void BindData(Level levelData, GameObject currentMenu, GameObject levelMenu)
        {
            m_CurrentMenu = currentMenu;
            m_LevelMenu = levelMenu;
            m_Level = levelData;
            m_IDText.text = $"ID: {levelData.ID}";
            m_NameText.text = levelData.Name;
        }

        public void OnView()
        {
            m_GameManager.SetCurrentLevel(m_Level);

            m_LevelMenu.SetActive(true);
        }
    }
}
