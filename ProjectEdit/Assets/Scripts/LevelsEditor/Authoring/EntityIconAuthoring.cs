using ProjectEdit.LevelsEditor.Components;
using Unity.Entities;
using UnityEngine;

namespace ProjectEdit.LevelsEditor.Authoring
{
    public class EntityIconAuthoring : MonoBehaviour
    {
        [SerializeField] private Sprite m_Value;

        private class EntityIconBaker : Baker<EntityIconAuthoring>
        {
            public override void Bake(EntityIconAuthoring authoring)
            {
                AddComponentObject(
                    GetEntity(TransformUsageFlags.None), 
                    new EntityIcon { Value = authoring.m_Value }
                );
            }
        }
    }
}