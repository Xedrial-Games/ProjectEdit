using UnityEngine.UIElements;

namespace ProjectEdit.UI
{
    public class InspectorPanel : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorPanel, UxmlTraits>
        {
        }

        public InspectorPanel() => RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);

        private void OnGeometryChanged(GeometryChangedEvent _)
        {
            UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }
    }
}