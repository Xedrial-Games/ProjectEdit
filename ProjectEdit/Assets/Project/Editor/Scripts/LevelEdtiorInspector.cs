using ProjectEdit;
using UnityEditor;
using UnityEngine;

namespace ArcaneNebulaEdtior
{
    [CustomEditor(typeof(LevelLoader))]
    public class LevelEdtiorInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            LevelLoader levelLoader = (LevelLoader)target;

            EditorGUILayout.LabelField("YAML");

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Load"))
                levelLoader.LoadYAML();

            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Compressed");

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Save"))
                levelLoader.SaveCompressed();

            if (GUILayout.Button("Load"))
                levelLoader.LoadCompressed();

            GUILayout.EndHorizontal();
        }
    }
}
