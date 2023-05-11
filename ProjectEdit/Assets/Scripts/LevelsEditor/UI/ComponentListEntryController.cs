using Unity.Entities;
using UnityEngine.UIElements;

namespace ProjectEdit.LevelsEditor.UI
{
    public class ComponentListEntryController
    {
        private ComponentType ComponentType { get; set; }

        private Label m_ComponentNameLabel;

        public void Initialize(VisualElement parentVisualElement) 
            => m_ComponentNameLabel = parentVisualElement.Q<Label>("component-name");

        public void SetComponentData(ComponentType componentType)
        {
            ComponentType = componentType;
            m_ComponentNameLabel.text = ComponentType.ToString();
        }
    }
}