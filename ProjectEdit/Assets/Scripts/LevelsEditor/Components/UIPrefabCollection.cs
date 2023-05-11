using Unity.Entities;

namespace ProjectEdit.LevelsEditor.Components
{
    public struct UIPrefabCollection : IBufferElementData
    {
        public Entity Prefab;
    }
}