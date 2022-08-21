using System.Collections;
using UnityEngine;

namespace ProjectEdit
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private int m_MaxHealth = 100;
        [SerializeField] private Transform m_Player;

        private int m_CurHealth;

        private Rigidbody2D m_RigidBody;
        private SpriteRenderer m_SpriteRenderer;

        private void Start()
        {
            m_CurHealth = m_MaxHealth;

            m_RigidBody = GetComponentInChildren<Rigidbody2D>();
            m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public void TakeDamage(int damage, Vector2 direction) => StartCoroutine(TakeDamageImpl(damage, direction));

        private IEnumerator TakeDamageImpl(int damage, Vector2 direction)
        {
            m_CurHealth -= damage;
            if (m_CurHealth <= 0)
            {
                Destroy(gameObject);
            }

            m_RigidBody.AddForce(direction * 20.0f, ForceMode2D.Impulse);

            for (short i = 0; i < 3; i++)
            {
                m_SpriteRenderer.color = Color.red;

                yield return new WaitForSeconds(0.1f);

                m_SpriteRenderer.color = Color.white;
            }
        }

        public void Flip()
        {
            if (m_Player.position.x > transform.position.x && transform.eulerAngles.y == 180.0f)
                transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            else if (m_Player.position.x < transform.position.x && transform.eulerAngles.y == 0.0f)
                transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
        }
    }
}
