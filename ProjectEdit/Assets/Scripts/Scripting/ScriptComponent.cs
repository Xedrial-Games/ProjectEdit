using Unity.Collections;
using Unity.Entities;

namespace ProjectEdit.Scripting
{
    public struct ScriptComponent : IComponentData
    {
        public FixedString32Bytes ScriptName;
    }
}