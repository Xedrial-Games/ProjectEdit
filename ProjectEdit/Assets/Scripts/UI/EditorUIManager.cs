using UnityEngine.UIElements;

namespace ProjectEdit.UI
{
    public class EditorUIManager : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<EditorUIManager, UxmlTraits>
        {
        }

        public EditorUIManager() => RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);

        private void OnGeometryChanged(GeometryChangedEvent _)
        {
            UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }
    }
}