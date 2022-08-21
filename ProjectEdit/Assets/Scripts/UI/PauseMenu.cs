using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace ProjectEdit.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject m_PauseMenu;

        private void Awake() => InputSystem.UI.Pause.performed += OnPauseClicked;
        private void OnDestroy() => InputSystem.UI.Pause.performed -= OnPauseClicked;

        public void OnPauseClicked(InputAction.CallbackContext _)
        {
            if (m_PauseMenu.activeSelf)
                Resume();
            else Pause();
        }

        public void Pause()
        {
            Time.timeScale = 0.0f;
            m_PauseMenu.SetActive(true);
        }

        public void Resume()
        {
            Time.timeScale = 1.0f;
            m_PauseMenu.SetActive(false);
        }

        public void Menu()
        {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene("MainMenu");
        }

        public void Restart()
        {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene("Level");
        }
    }
}
