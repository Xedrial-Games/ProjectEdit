using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

namespace ProjectEdit.UI
{
    public class EditorLevelMenu : MonoBehaviour
    {
        public static EditorLevelMenu Instance { get; private set; }

        [SerializeField] private TextMeshProUGUI m_LevelName;

        private GameManager m_GameManager;
        private EditorMenu m_EditorMenu;
        private Serializer m_Serializer;

        private EditorLevelMenu() => Instance = this;

        private void Awake()
        {
            m_GameManager = GameManager.Instance;
            m_Serializer = new(Serializer.LevelsPath);
            m_EditorMenu = EditorMenu.Instance;
        }

        private void OnEnable() => m_LevelName.text = m_GameManager.CurrentLevel.Name;

        public void PlayLevel() => SceneManager.LoadScene("Level");

        public void EditLevel() => SceneManager.LoadScene("Editor");

        public void DeleteLevel()
        {
            m_Serializer.DeleteLevel(m_GameManager.CurrentLevel);
            GameManager.Instance.SetCurrentLevel(Level.Null);

            m_EditorMenu.gameObject.SetActive(true);
            m_EditorMenu.ReloadData();
            gameObject.SetActive(false);
        }

        public async void UploadLevel()
        {
            Level level = m_GameManager.CurrentLevel;
            string data = m_Serializer.SerializeTilesDataCompressed(level.TilesData);
            DirectoryInfo di = m_Serializer.GetLevelPath(level);
            string responseString = await Database.UploadLevel(level.Name, di);
            Debug.Log(responseString);
        }
    }
}
