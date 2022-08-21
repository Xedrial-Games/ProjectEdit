using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace ProjectEdit.UI
{
    public class OnlineLevelMenu : MonoBehaviour
    {
        public static OnlineLevelMenu Instance { get; private set; }

        [SerializeField] private TextMeshProUGUI m_LevelName;

        private GameManager m_GameManager;

        private OnlineLevelMenu() => Instance = this;

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

            gameObject.SetActive(false);
        }
    }
}
