using MoonSharp.Interpreter;
using UnityEngine.UIElements;

namespace ProjectEdit.UI.Properties
{
    public class TextField : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TextField, UxmlTraits>
        {
        }

        private Table m_Exports;
        private string m_Key;

        public TextField() => RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);

        private void OnGeometryChanged(GeometryChangedEvent _)
        {
            UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }
    }
}