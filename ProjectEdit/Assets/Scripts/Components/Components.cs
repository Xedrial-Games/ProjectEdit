using Unity.Entities;
using Unity.Collections;

namespace ProjectEdit.Components
{
    public struct ScriptComponent : IComponentData
    {
        public FixedString32Bytes Script;
    }
}
