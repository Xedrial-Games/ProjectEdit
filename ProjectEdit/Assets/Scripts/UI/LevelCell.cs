using UnityEngine;
using TMPro;

namespace ProjectEdit.UI
{
    public class LevelCell : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_IDText;

        private Level m_Level;
        private GameManager m_GameManager;
        private GameObject m_LevelMenu;
        private GameObject m_EditorMenu;

        private void Awake() => m_GameManager = GameManager.Instance;

        private void Start()
        {
            m_LevelMenu = EditorLevelMenu.Instance.gameObject;
            m_EditorMenu = EditorMenu.Instance.gameObject;
        }

        public void BindData(Level levelData)
        {
            m_Level = levelData;
            m_IDText.text = $"ID: {levelData.ID}";
            m_NameText.text = levelData.Name;
        }

        public void OnView()
        {
            m_GameManager.SetCurrentLevel(m_Level);

            m_LevelMenu.SetActive(true);
            m_EditorMenu.SetActive(false);
        }
    }
}
