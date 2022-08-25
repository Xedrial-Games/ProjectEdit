using System.Collections.Generic;

using UnityEngine;

using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;
using UEntity = Unity.Entities.Entity;

using ProjectEdit.Components;

namespace ProjectEdit.Entities
{
    public class EntitiesManager : MonoBehaviour
    {
        // The entity prefab game object.
        [SerializeField] private GameObject m_EntityGameObjectPrefab;

        private EntityManager m_EntityManager;
        private World m_World;
        private BlobAssetStore m_BlobAssetStore;

        // A list of all the entities in the scene.
        private readonly List<Entity> m_EntityInstances = new();

        // The entity prefab created from the game object.
        private UEntity m_EntityPrefab;

        private void Awake()
        {
            // Get References
            m_World = World.DefaultGameObjectInjectionWorld;
            m_EntityManager = m_World.EntityManager;
            m_BlobAssetStore = new();

            // Ready the entities list
            m_EntityInstances.Clear();
            
            // Ready the Entity prefab
            GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(m_World, m_BlobAssetStore);
            m_EntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy
                (m_EntityGameObjectPrefab, settings);

            // Disable receive shadows (maybe for performance but I am not sure if it affects)
            var renderMesh = m_EntityManager.GetSharedComponentData<RenderMesh>(m_EntityPrefab);
            renderMesh.receiveShadows = false;
            m_EntityManager.SetSharedComponentData(m_EntityPrefab, renderMesh);
        }

        /// <summary>
        /// Creates a new entity and adds it.
        /// </summary>
        /// <returns>The newly created entity</returns>
        public Entity CreateEntity()
        {
            Entity entity = new(m_EntityManager.Instantiate(m_EntityPrefab),
                m_EntityManager);
            m_EntityInstances.Add(entity);

            entity.SetComponentData(new MainTexMaterialProperty
            {
                Value = new float2x4(1f)
            });

            return entity;
        }

        /// <summary>
        /// Destroys an entity and removes it.
        /// </summary>
        /// <param name="entity">The entity to remove</param>
        /// <returns>whether the operation was successful or not</returns>
        public bool DestroyEntity(Entity entity)
        {
            if (m_EntityInstances.Contains(entity))
            {
                m_EntityInstances.Remove(entity);
                m_EntityManager.DestroyEntity(entity);
                return true;
            }
            
            Debug.LogError("EntityCreator.DestroyEntity-> Invalid entity specified!", gameObject);
            return false;
        }

        private void OnDestroy()
        {
            m_BlobAssetStore.Dispose();
        }
    }
}
