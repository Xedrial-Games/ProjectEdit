using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

namespace ProjectEdit
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance;

        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_NPCDialogueText;
        [SerializeField] private TextMeshProUGUI m_StoryDialogueText;

        private Queue<string> m_Sentences;

        private TextMeshProUGUI m_ActiveText;
        private Animator m_Animator;

        bool m_DialogueEnded = true;
        bool m_Dialogue = false;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != null)
                Destroy(this);
        }

        private void Start() => m_Sentences = new Queue<string>();

        private void Update()
        {
            if (m_Dialogue)
            {
                if (Input.GetKeyDown(KeyCode.Z) && m_DialogueEnded)
                    DisplayNextSentece();
                if (Input.GetKeyDown(KeyCode.X) && !m_DialogueEnded)
                    m_DialogueEnded = true;
            }
        }

        public void StartDialogue(Dialogue dialogue)
        {
            if (!m_DialogueEnded)
                return;

            m_Dialogue = true;

            switch (dialogue.DialogueType)
            {
                case DialogueType.Story:
                    m_ActiveText = m_StoryDialogueText;
                    m_Animator = m_StoryDialogueText.GetComponent<Animator>();
                    break;
                case DialogueType.NPC:
                    m_ActiveText = m_NPCDialogueText;
                    m_Animator = m_NPCDialogueText.GetComponentInParent<Animator>();
                    break;
            }

            m_Animator.SetBool("IsOpen", true);
            m_NameText.text = dialogue.Name;

            m_Sentences.Clear();

            foreach (string sentence in dialogue.Sentences)
                m_Sentences.Enqueue(sentence);

            DisplayNextSentece();
        }

        public void DisplayNextSentece()
        {
            if (m_Sentences.Count == 0)
            {
                EndDualogue();
                return;
            }

            string sentence = m_Sentences.Dequeue();
            m_DialogueEnded = false;
            StopAllCoroutines();
            StartCoroutine(TypeSentence(sentence));
        }

        private IEnumerator TypeSentence(string sentece)
        {
            m_ActiveText.text = string.Empty;
            foreach (char let in sentece)
            {
                if (m_DialogueEnded)
                {
                    m_ActiveText.text = sentece;
                    break;
                }

                m_ActiveText.text += let;
                yield return new WaitForSeconds(0.02f);
            }

            m_DialogueEnded = true;
        }

        private void EndDualogue()
        {
            m_Animator.SetBool("IsOpen", false);
            Debug.Log("End of conversation");
            m_Dialogue = false;
        }
    }
}
