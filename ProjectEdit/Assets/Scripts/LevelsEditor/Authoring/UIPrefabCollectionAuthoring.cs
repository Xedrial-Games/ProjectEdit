using ProjectEdit.LevelsEditor.Components;
using Unity.Entities;
using UnityEngine;

namespace ProjectEdit.LevelsEditor.Authoring
{
    public class UIPrefabCollectionAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject[] m_EntitiesPrefabs;
        
        private class UIPrefabCollectionBaker : Baker<UIPrefabCollectionAuthoring>
        {
            public override void Bake(UIPrefabCollectionAuthoring authoring)
            {
                if (authoring.m_EntitiesPrefabs == null)
                    return;
                
                var buffer = AddBuffer<UIPrefabCollection>(GetEntity(TransformUsageFlags.None));
                foreach (GameObject prefab in authoring.m_EntitiesPrefabs)
                {
                    buffer.Add(new UIPrefabCollection
                    {
                        Prefab = GetEntity(prefab, TransformUsageFlags.None)
                    });
                }
            }
        }
    }
}