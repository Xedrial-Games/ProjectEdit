using UnityEngine;

namespace ProjectEdit
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private GameObject m_PlayerContainer;

        private void Start()
        {
            Instantiate(m_PlayerContainer, new(0.5f, 0.5f, 0.0f), Quaternion.identity);
        }
    }
}
