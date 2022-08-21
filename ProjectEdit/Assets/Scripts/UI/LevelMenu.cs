using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

namespace ProjectEdit.UI
{
    public class LevelMenu : MonoBehaviour
    {
        public static LevelMenu Instance { get; private set; }

        [SerializeField] private TextMeshProUGUI m_LevelName;

        private GameManager m_GameManager;

        private LevelMenu() => Instance = this;

        private void Awake()
        {
            m_GameManager = GameManager.Instance;
        }

        private async void OnEnable()
        {
            Level level = m_GameManager.CurrentLevel;
            m_LevelName.text = level.Name;
            level = await m_GameManager.OnlineSerializer.GetOnlineLevel(level.ID);
            m_GameManager.SetCurrentLevel(level);
        }

        public void PlayLevel() => SceneManager.LoadScene("Level");

        public void DeleteLevel()
        {
            m_GameManager.OnlineSerializer.DeleteLevel(m_GameManager.CurrentLevel);
            GameManager.Instance.SetCurrentLevel(Level.Empty);
        }
    }
}
