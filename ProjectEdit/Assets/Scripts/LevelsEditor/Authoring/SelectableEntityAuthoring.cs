using ProjectEdit.LevelsEditor.Components;
using Unity.Entities;
using UnityEngine;

namespace ProjectEdit.LevelsEditor.Authoring
{
    public class SelectableEntityAuthoring : MonoBehaviour
    {
        public class SelectableEntityBaker : Baker<SelectableEntityAuthoring>
        {
            public override void Bake(SelectableEntityAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SelectableEntity());
            }
        }
    }
}