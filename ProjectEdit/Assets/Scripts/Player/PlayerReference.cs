using UnityEngine;

namespace ProjectEdit
{
    public class PlayerReference : MonoBehaviour
    {
        private PlayerMotor m_Motor;
        private PlayerController m_Controller;
        private PlayerCombat m_Combat;

        private void Start()
        {
            m_Combat = GetComponentInParent<PlayerCombat>();
            m_Controller = GetComponentInParent<PlayerController>();
            m_Motor = GetComponentInParent<PlayerMotor>();
        }

        public void PerformAttack() => m_Combat.PerformAttack();
    }
}
