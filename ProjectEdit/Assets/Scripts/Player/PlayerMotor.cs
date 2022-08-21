using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectEdit
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMotor : MonoBehaviour
    {
        public bool FacingRight { get { return m_FacingRight; } }

        [Header("Movement")]
        [SerializeField] private float m_MoveSpeed = 10f;
        [SerializeField] private float m_Acceleration = 7f;
        [SerializeField] private float m_Decceleration = 7f;
        [SerializeField] private float m_VelocityPower = 0.9f;

        [Space()]
        [SerializeField] private float m_FrictionAmount = 0.2f;

        [Header("Jump")]
        [SerializeField] private float m_JumpForce = 80.0f;
        [SerializeField] private float m_JumpCutMultiplier = 0.5f;

        [Space()]
        [SerializeField] private float m_JumpCoyoteTime = 0.1f;
        [SerializeField] private float m_JumpBufferTime = 0.1f;

        [Space()]
        [SerializeField] private float m_GravityScale = 1f;
        [SerializeField] private float m_FallGravityMultiplier = 2f;

        [Header("Wall Jump")]
        [SerializeField] private float m_WallSlidingSpeed = 5f;
        [SerializeField] private Vector2 m_WallForce = new Vector2(20f, 15f);
        [SerializeField] private float m_WallJumpTime = 0.1f;

        [Header("Dash")]
        [SerializeField] private float m_DashForce = 50f;
        [SerializeField] private float m_DashTime = 0.1f;
        [SerializeField] private float m_NextDashTime = 0.1f;

        [Header("Checks")]
        [SerializeField] private Transform m_GroundCheck;
        [SerializeField] private Vector2 m_GroundCheckSize = new Vector2(0.5f, 0.02f);
        [SerializeField] private LayerMask m_GroundLayer;

        [Space()]
        [SerializeField] private Transform m_WallCheck;
        [SerializeField] private float m_WallCheckRadius = 0.3f;
        [SerializeField] private LayerMask m_WallLayer;

        // Jump
        private bool m_IsJumping = false;
        private float m_LastJumpTime = 0.0f;
        private bool m_JumpInputReleased = false;

        // Wall Jump
        private bool m_WallJumping = false;

        // Dash
        private bool m_IsDashing = false;
        private bool m_CanDash = true;
        private bool m_IsDashTime = false;

        // Ground Check
        private bool m_IsGrounded = false;
        private bool m_WasGrounded = false;
        private float m_LastGroundedTime = 0.0f;

        // Wall Check
        private bool m_IsTouchingWall = false;
        private bool m_IsSliding = false;

        // Flip
        private bool m_FacingRight = true;

        private Rigidbody2D m_Rigidbody;
        private Animator m_Animator;
        private PlayerCombat m_PlayerCombat;

        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody2D>();
            m_Animator = GetComponentInChildren<Animator>();
            m_PlayerCombat = GetComponent<PlayerCombat>();
        }

        private void FixedUpdate()
        {
            m_WasGrounded = m_IsGrounded;

            if (Physics2D.OverlapBox(m_GroundCheck.position, m_GroundCheckSize, 0.0f, m_GroundLayer))
            {
                m_IsGrounded = true;
                m_LastGroundedTime = m_JumpCoyoteTime;
            }
            else m_IsGrounded = false;

            m_IsTouchingWall = Physics2D.OverlapCircle(m_WallCheck.position, m_WallCheckRadius, m_WallLayer);
        }

        public void Move(float move)
        {
            // Movement
            float targetSpeed = move * m_MoveSpeed;
            float speedDiff = targetSpeed - m_Rigidbody.velocity.x;
            float acelRate = Mathf.Abs(targetSpeed) > 0.01f ? m_Acceleration : m_Decceleration;
            float movement = Mathf.Pow(Mathf.Abs(speedDiff) * acelRate, m_VelocityPower) * Mathf.Sign(speedDiff);

            if (!m_IsDashing && !m_PlayerCombat.IsAttacking)
                m_Rigidbody.AddForce(movement * Vector2.right);

            // Firction
            if (Mathf.Abs(move) > 0.01f)
            {
                float amount = Mathf.Min(Mathf.Abs(m_Rigidbody.velocity.x), Mathf.Abs(m_FrictionAmount));
                amount *= Mathf.Sign(m_Rigidbody.velocity.x);
                m_Rigidbody.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
            }

            // Jump
            if (m_JumpInputReleased)
                m_IsJumping = false;

            if (m_LastGroundedTime > 0.0f && m_LastJumpTime > 0.0f && !m_IsJumping)
                Jump();

            m_LastGroundedTime -= Time.deltaTime;
            m_LastJumpTime -= Time.deltaTime;

            // Wall Jumping
            m_IsSliding = m_IsTouchingWall && !m_IsGrounded && move != 0.0f;

            if (m_IsSliding)
            {
                Vector2 rbv = m_Rigidbody.velocity;
                m_Rigidbody.velocity = new Vector2(rbv.x, Mathf.Clamp(rbv.y, -m_WallSlidingSpeed, float.MaxValue));
            }

            if (m_WallJumping)
                WallJump();

            // Dashing
            if (m_IsDashing)
                Dash();

            if (m_IsGrounded && m_IsDashTime)
                m_CanDash = true;

            // Gravity Scaling
            if (!m_IsDashing)
            {
                if (m_Rigidbody.velocity.y < 0.0f)
                    m_Rigidbody.gravityScale = m_GravityScale * m_FallGravityMultiplier;
                else m_Rigidbody.gravityScale = m_GravityScale;
            }

            // Fliping the player
            if (!m_PlayerCombat.IsAttacking)
            {
                if (m_FacingRight && move < 0.0f) Flip();
                else if (!m_FacingRight && move > 0.0f) Flip();
            }
        }

        #region Jump

        public void OnJump(InputAction.CallbackContext context)
        {
            m_LastJumpTime = m_JumpBufferTime;
        }

        private void Jump()
        {
            m_Rigidbody.AddForce(Vector2.up * m_JumpForce, ForceMode2D.Impulse);
            m_LastGroundedTime = 0.0f;
            m_LastJumpTime = 0.0f;
            m_IsJumping = true;
            m_JumpInputReleased = false;
        }

        public void OnJumpStop(InputAction.CallbackContext context)
        {
            if (m_Rigidbody.velocity.y > 0.0f && m_IsJumping)
                m_Rigidbody.AddForce((1 - m_JumpCutMultiplier) * m_Rigidbody.velocity.y * Vector2.down, ForceMode2D.Impulse);

            m_JumpInputReleased = true;
            m_LastJumpTime = 0.0f;
        }

        #endregion

        #region Wall Jump

        public void OnWallJump(InputAction.CallbackContext context)
        {
            if (m_IsSliding)
            {
                m_WallJumping = true;
                Invoke("OnWallJumpStop", m_WallJumpTime);
            }
        }

        private void WallJump()
        {
            m_Rigidbody.velocity = new Vector2(m_WallForce.x * (m_FacingRight ? -1 : 1), m_WallForce.y);
        }

        private void OnWallJumpStop()
        {
            m_WallJumping = false;
        }

        #endregion

        #region Dash

        public void OnDash(InputAction.CallbackContext context)
        {
            if (!m_IsDashing && m_CanDash)
            {
                m_IsDashing = true;
                m_CanDash = false;
                m_IsDashTime = false;
                m_Rigidbody.gravityScale = 0;
                m_Rigidbody.velocity = Vector2.zero;
                StartCoroutine(StopDash());
            }
        }

        private void Dash()
        {
            m_Rigidbody.AddForce(transform.right * m_DashForce * Time.fixedDeltaTime, ForceMode2D.Impulse);
        }

        private IEnumerator StopDash()
        {
            yield return new WaitForSeconds(m_DashTime);

            m_IsDashing = false;
            m_Rigidbody.gravityScale = m_GravityScale;
            m_Rigidbody.velocity = Vector2.zero;

            yield return new WaitForSeconds(m_NextDashTime);

            m_IsDashTime = true;
        }

        #endregion

        public void OnAttack(float moveY, float attackForce)
        {
            if (Mathf.Abs(moveY) < 0.5f)
                m_Rigidbody.AddForce(-transform.right * attackForce, ForceMode2D.Impulse);
            else m_Rigidbody.AddForce(attackForce * Mathf.Sign(moveY) * -transform.up, ForceMode2D.Impulse);
        }

        public void Animate(float move)
        {
            m_Animator.SetFloat("Speed", Mathf.Abs(move));
            m_Animator.SetFloat("vSpeed", m_Rigidbody.velocity.y);
            m_Animator.SetBool("IsGrounded", m_IsGrounded);
        }

        private void Flip()
        {
            transform.rotation = transform.eulerAngles.y == 0.0f ? Quaternion.Euler(0.0f, 180.0f, 0.0f) : Quaternion.Euler(0.0f, 0.0f, 0.0f);
            m_FacingRight = !m_FacingRight;
        }
    }
}
