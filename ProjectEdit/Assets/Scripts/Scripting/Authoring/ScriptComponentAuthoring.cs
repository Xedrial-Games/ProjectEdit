using Unity.Entities;
using UnityEngine;

namespace ProjectEdit.Scripting.Authoring
{
    public class ScriptComponentAuthoring : MonoBehaviour
    {
        [SerializeField] private string m_ScriptName;

        public class ScriptComponentBaker : Baker<ScriptComponentAuthoring>
        {
            public override void Bake(ScriptComponentAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ScriptComponent { ScriptName = authoring.m_ScriptName });
            }
        }
    }
}