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

            ScriptEngine engine = (ScriptEngine)target;
            if (GUILayout.Button("Load Lua"))
                engine.LoadLua();

            if (GUILayout.Button("Execute Lua"))
                engine.ExecuteLua();            

            EditorGUILayout.EndHorizontal();
        }
    }
}
