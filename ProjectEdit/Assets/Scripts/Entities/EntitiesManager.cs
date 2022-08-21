using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace ProjectEdit.Entities
{
    public class EntitiesManager : MonoBehaviour
    {
        // The entity prefab game object.
        [SerializeField] private GameObject m_EntityGameObjectPrefab;

        private EntityManager m_EntityManager;
        private World m_World;

        // A list of all the entities in the scene.
        private readonly List<EntityInstance> m_EntityInstances = new();

        // The entity prefab created from the game object.
        private Entity m_EntityPrefab;

        private void Awake()
        {
            // Get References
            m_World = World.DefaultGameObjectInjectionWorld;
            m_EntityManager = m_World.EntityManager;

            // Ready the entities list
            m_EntityInstances.Clear();
            
            // Ready the Entity prefab
            var settings = GameObjectConversionSettings.FromWorld(m_World, null);
            m_EntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy
                (m_EntityGameObjectPrefab, settings);

            // Disable receive shadows (maybe for performance but I am not sure if it affects)
            var rm = m_EntityManager.GetSharedComponentData<RenderMesh>(m_EntityPrefab);
            rm.receiveShadows = false;
            m_EntityManager.SetSharedComponentData(m_EntityPrefab, rm);
        }

        /// <summary>
        /// Creates a new entity and adds it.
        /// </summary>
        /// <returns>The newly created entity</returns>
        public EntityInstance CreateEntity()
        {
            EntityInstance entityInstance = new(m_EntityManager.Instantiate(m_EntityPrefab),
                m_EntityManager);
            m_EntityInstances.Add(entityInstance);

            return entityInstance;
        }

        /// <summary>
        /// Destroys an entity and removes it.
        /// </summary>
        /// <param name="entityInstance">The entity to remove</param>
        /// <returns>whether the operation was successful or not</returns>
        public bool DestroyEntity(EntityInstance entityInstance)
        {
            if (m_EntityInstances.Contains(entityInstance))
            {
                m_EntityInstances.Remove(entityInstance);
                m_EntityManager.DestroyEntity(entityInstance.Entity);
                return true;
            }
            
            Debug.LogError("EntityCreator.DestroyEntity-> Invalid entity specified!",
                gameObject);
            return false;
        }
    }
}
