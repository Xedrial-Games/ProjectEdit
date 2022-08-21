using UnityEngine;

namespace ProjectEdit
{
    public static class InputSystem
    {
        public static InputActions.PlayerActions Player { get { return s_Player; } }
        private static InputActions.PlayerActions s_Player;

        public static InputActions.EditorActions Editor { get { return s_Editor; } }
        private static InputActions.EditorActions s_Editor;

        public static InputActions.UIActions UI { get { return s_UI; } }
        private static InputActions.UIActions s_UI;

        private static InputActions s_Instance;

        public static void Init()
        {
            s_Instance = new InputActions();
            s_Player = s_Instance.Player;
            s_Editor = s_Instance.Editor;
            s_UI = s_Instance.UI;

            UI.Enable();
        }

        public static void Shutdown()
        {
            s_Instance.Disable();
        }
    }
}
