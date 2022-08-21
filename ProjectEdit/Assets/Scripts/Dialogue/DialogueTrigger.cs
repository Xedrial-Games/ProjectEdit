using UnityEngine;

namespace ProjectEdit
{
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField] private Dialogue m_Dialogue;

        private DialogueManager m_DialogueManager;

        private void Start() => m_DialogueManager = DialogueManager.Instance;

        public void TriggerDialogue()
        {
            m_DialogueManager.StartDialogue(m_Dialogue);
        }
    }
}
