using UnityEngine;

namespace ProjectEdit
{
    public class EnemyAttack : MonoBehaviour
    {
        [SerializeField] private GameObject m_Projectile;
        [SerializeField] private Transform m_Target;
        [SerializeField] private int m_Attacks = 4;
        [SerializeField] private float m_AttackRate = 4.0f;
        [SerializeField] private float m_Offset = 90f;

        private bool m_Attack = false;
        private int m_AttackIndex = 0;

        private EnemyAI m_EnemyAI;

        private void Start()
        {
            m_EnemyAI = GetComponent<EnemyAI>();
        }

        private void Update()
        {
            if (m_EnemyAI.ReachedEndOfPath && !m_Attack)
            {
                m_Attack = true;
                InvokeRepeating("Attack", 0.2f, m_AttackRate);
            }
            else if (!m_EnemyAI.ReachedEndOfPath)
                m_Attack = false;

            if (m_AttackIndex >= m_Attacks)
            {
                CancelInvoke("Attack");
                m_AttackIndex = 0;
            }
        }

        private void Attack()
        {
            Vector3 direction = (transform.position - m_Target.position).normalized;
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            Instantiate(m_Projectile, transform.position, Quaternion.Euler(new Vector3(0f, 0f, -angle - m_Offset)));
            m_AttackIndex++;
        }
    }
}
