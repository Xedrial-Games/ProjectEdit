using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectEdit.UI
{
    public class MenuManager : MonoBehaviour
    {
        public void PlayGame()
        {
            SceneManager.LoadScene("Level");
        }
    }
}
