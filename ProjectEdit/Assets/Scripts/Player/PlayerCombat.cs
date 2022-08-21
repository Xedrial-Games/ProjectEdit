using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectEdit
{
    public class PlayerCombat : MonoBehaviour
    {
        public float MoveY { get { return m_MoveY; } set { m_MoveY = value; } }

        [HideInInspector] public bool IsAttacking { get { return m_NextAttackTime > Time.time; } }

        [SerializeField] private LayerMask m_EnemiesLayer;
        [SerializeField] private Attack[] m_ComboAttacks;

        private PlayerMotor m_Motor;

        private float m_MoveY = 0.0f;
        private float m_NextAttackTime = 0.0f;
        private float m_AttackExitTime = 0.0f;
        private int m_CurrentAttackIndex = 0;

        private void Start()
        {
            m_Motor = GetComponentInChildren<PlayerMotor>();
        }

        private void Update()
        {
            if (m_AttackExitTime > 0)
                m_AttackExitTime -= Time.deltaTime;
            else
            {
                m_AttackExitTime = 0;
                m_CurrentAttackIndex = 0;
            }
        }

        public void SetAttack(InputAction.CallbackContext context)
        {
            if (Time.time > m_NextAttackTime)
            {
                Attack attack = m_ComboAttacks[m_CurrentAttackIndex];

                m_AttackExitTime = attack.AttackAnimation.length + 0.1f;
                m_NextAttackTime = Time.time + attack.AttackAnimation.length - 0.1f;

                Quaternion rotation;
                Vector3 position;
                if (Mathf.Abs(m_MoveY) > 0.5f)
                {
                    rotation = m_MoveY > 0.01f ? Quaternion.Euler(0.0f, 0.0f, 90f) : Quaternion.Euler(0.0f, 0.0f, 270f);
                    position = attack.VAttackPoint.position;
                }
                else
                {
                    rotation = m_Motor.FacingRight ? Quaternion.identity : Quaternion.Euler(0.0f, 180f, 0.0f);
                    position = attack.AttackPoint.position;
                }

                Destroy(Instantiate(attack.AttackPrefab, position, rotation), m_AttackExitTime);

                m_Motor.OnAttack(m_MoveY, attack.AttackForce);
                PerformAttack();
            }
        }

        public void PerformAttack()
        {
            Attack attack = m_ComboAttacks[m_CurrentAttackIndex];

            if (attack.DamagePoint)
            {
                Vector3 pos, direction;
                if (Mathf.Abs(m_MoveY) > 0.5f)
                {
                    pos = attack.VDamagePoint.position;
                    direction = new Vector3(0.0f, Mathf.Sign(m_MoveY), 0.0f);
                }
                else
                {
                    pos = attack.DamagePoint.position;
                    direction = new Vector3(m_Motor.FacingRight ? 1.0f : -1.0f, 0.0f, 0.0f);
                }

                Collider2D[] enemies = Physics2D.OverlapCircleAll(pos, attack.AttackRadius, m_EnemiesLayer);

                foreach (Collider2D eCollider in enemies)
                    eCollider.GetComponent<Enemy>()?.TakeDamage(40, direction);
            }

            m_CurrentAttackIndex++;

            if (m_CurrentAttackIndex >= m_ComboAttacks.Length)
                m_CurrentAttackIndex = 0;
        }

        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < m_ComboAttacks.Length; i++)
            {
                if (m_ComboAttacks[i].DamagePoint)
                {
                    Gizmos.DrawWireSphere(m_ComboAttacks[m_CurrentAttackIndex].DamagePoint.position,
                        m_ComboAttacks[m_CurrentAttackIndex].AttackRadius);

                    Gizmos.DrawWireSphere(m_ComboAttacks[m_CurrentAttackIndex].VDamagePoint.position,
                        m_ComboAttacks[m_CurrentAttackIndex].AttackRadius);
                }
            }
        }
    }

    [System.Serializable]
    public class Attack
    {
        public AnimationClip AttackAnimation;
        public GameObject AttackPrefab;
        public Transform AttackPoint;
        public Transform DamagePoint;
        public Transform VAttackPoint;
        public Transform VDamagePoint;
        public float AttackForce = 20f;
        public float AttackRadius = 0.3f;
    }
}
