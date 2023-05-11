using ProjectEdit.Entities;
using Unity.Entities;
using UnityEngine;

namespace ProjectEdit.Authoring
{
    public class RuntimeEntityAuthoring : MonoBehaviour
    {
        public class RuntimeEntityBaker : Baker<RuntimeEntityAuthoring>
        {
            public override void Bake(RuntimeEntityAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);
                AddComponent<RuntimeEntity>(entity);
            }
        }
    }
}