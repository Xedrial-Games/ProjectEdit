using System.IO;
using UnityEngine;

namespace ProjectEdit
{
    [DefaultExecutionOrder(-2)]
    public class GameManager : Singelton<GameManager>
    {
        public ref Serializer Serializer => ref m_Serializer;

        public ref Serializer OnlineSerializer => ref m_OnlineSerializer;

        public Level CurrentLevel { get; private set; }

        private Serializer m_Serializer;
        private Serializer m_OnlineSerializer;

        protected override void Awake()
        {
            base.Awake();

            DontDestroyOnLoad(this);

            CurrentLevel = Level.Null;

            m_Serializer = new Serializer(Serializer.LevelsPath);
            m_OnlineSerializer = new Serializer(Serializer.OnlineLevelsPath);

            InputSystem.Init();
        }

        public void SetCurrentLevel(Level level) => CurrentLevel = level;

        private void OnDisable()
        {
            InputSystem.Shutdown();
        }
    }
}
