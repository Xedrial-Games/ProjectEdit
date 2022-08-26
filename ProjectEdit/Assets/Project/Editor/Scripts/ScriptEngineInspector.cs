using UnityEngine;
using UnityEditor;
using ProjectEdit.Scripting;

namespace ProjectE_Editor
{
    [CustomEditor(typeof(ScriptEngine))]
    public class ScriptEngineInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Load Lua"))
                ScriptEngine.LoadLua();

            if (GUILayout.Button("Execute Lua"))
                ScriptEngine.ExecuteLua();            

            EditorGUILayout.EndHorizontal();
        }
    }
}
