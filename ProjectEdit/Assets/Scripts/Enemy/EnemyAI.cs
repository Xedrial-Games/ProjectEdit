using Pathfinding;
using UnityEngine;

namespace ProjectEdit
{
    public class EnemyAI : MonoBehaviour
    {
        public bool ReachedEndOfPath { get { return m_ReachedEndOfPath; } }

        [SerializeField] private Transform[] m_Targets;
        [SerializeField] private Transform m_Center;

        [SerializeField] private float m_Speed = 200f;
        [SerializeField] private float m_NextWaypointDistance = 3f;
        [SerializeField] private float m_NextUpdateTime = 3f;
        [SerializeField] private float m_MinLength = 3f;
        [SerializeField] private float m_MaxLength = 10f;

        private Path m_Path;
        private int m_CurrentWaypoint;
        private bool m_ReachedEndOfPath;
        private int m_LastIndex;

        private Seeker m_Seeker;
        private Rigidbody2D m_Rigidbody;

        private void Start()
        {
            if (m_Targets.Length == 0)
                Debug.LogError("No targets specified");

            m_Seeker = GetComponent<Seeker>();
            m_Rigidbody = GetComponent<Rigidbody2D>();

            InvokeRepeating(nameof(UpdatePath), 0.0f, m_NextUpdateTime);
        }

        private void UpdatePath()
        {
            if (m_Targets.Length == 0)
                return;

            if (m_Seeker.IsDone())
                m_Seeker.StartPath(m_Rigidbody.position, (Vector3)GetRandomPoint() + m_Center.position, OnPathComplete);
        }

        private void OnPathComplete(Path p)
        {
            if (p.error)
            {
                Debug.LogError(p.errorLog);
                return;
            }

            m_Path = p;
            m_CurrentWaypoint = 0;
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (m_Path == null)
                return;

            if (m_CurrentWaypoint >= m_Path.vectorPath.Count)
            {
                m_ReachedEndOfPath = true;
                return;
            }
            else m_ReachedEndOfPath = false;

            Vector2 direction = ((Vector2)m_Path.vectorPath[m_CurrentWaypoint] - m_Rigidbody.position).normalized;
            Vector2 force = m_Speed * Time.fixedDeltaTime * direction;

            m_Rigidbody.AddForce(force);

            float distance = Vector2.Distance(m_Rigidbody.position, m_Path.vectorPath[m_CurrentWaypoint]);
            if (distance < m_NextWaypointDistance)
            {
                m_CurrentWaypoint++;
            }
        }

        private Vector2 GetRandomPosition()
        {
            if (m_Targets.Length == 1)
                return m_Targets[0].position;

            int randomIndex;

            do { randomIndex = Random.Range(0, m_Targets.Length); }
            while (randomIndex == m_LastIndex);

            m_LastIndex = randomIndex;

            return m_Targets[randomIndex].position;
        }

        public void Hello()
        {
            Debug.Log("YES");
        }

        private Vector2 GetRandomPoint()
        {
            float randomLength = Random.Range(m_MinLength, m_MaxLength);
            float randomAngle = Random.Range(0.0f, Mathf.PI);

            return new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * randomLength;
        }
    }
}
