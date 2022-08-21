using UnityEngine;

namespace ProjectEdit
{
    public class FollowBehaviour : StateMachineBehaviour
    {
        [SerializeField] private float m_Speed = 3.0f;
        [SerializeField] private float m_AttackRange = 1.0f;

        private Transform m_Player;
        private Rigidbody2D m_Rigidbody;
        private Enemy m_Enemy;

        //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_Player = GameObject.FindGameObjectWithTag("Player")?.transform;
            m_Rigidbody = animator.GetComponent<Rigidbody2D>();
            m_Enemy = animator.GetComponent<Enemy>();
        }

        //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_Enemy.Flip();

            if (Vector2.Distance(m_Player.position, m_Rigidbody.position) <= m_AttackRange)
            {
                animator.SetTrigger("Attack");
                animator.SetBool("Attacking", true);
            }
            else
            {
                Vector2 targetPos = new Vector2(m_Player.position.x, m_Rigidbody.position.y);
                Vector2 newPos = Vector2.MoveTowards(m_Rigidbody.position, targetPos, m_Speed * Time.fixedDeltaTime);
                m_Rigidbody.MovePosition(newPos);
            }
        }

        //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.ResetTrigger("Attack");
        }
    }
}
