using System.Collections.Generic;
using ProjectEdit.Components;
using UnityEngine;
using Unity.Entities;

namespace ProjectEdit.Entities
{
    public class EntitiesManager : MonoBehaviour
    {
        // The entity prefab game object.
        [SerializeField] private GameObject m_EntityGameObjectPrefab;

        public static EntityManager EntityManager { get; private set; }
        private World m_World;
        private BlobAssetStore m_BlobAssetStore;

        // A list of all the entities in the scene.
        private static readonly List<Entity> s_Entities = new();

        // The entity prefab created from the game object.
        private static Entity s_EntityPrefab;

        private void Awake()
        {
            // Get References
            m_World = World.DefaultGameObjectInjectionWorld;
            EntityManager = m_World.EntityManager;
            m_BlobAssetStore = new BlobAssetStore();

            // Ready the entities list
            s_Entities.Clear();
            
            // Ready the Entity prefab
            GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(m_World, m_BlobAssetStore);
            s_EntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy
                (m_EntityGameObjectPrefab, settings);

            Entity entity = CreateEntity();
            EntityManager.AddComponent<ScriptComponent>(entity);
            EntityManager.SetComponentData(entity, new ScriptComponent { Script = "MoveSquare.lua" });
            
            Scripting.ScriptsSystem.AddScript(entity);
        }

        /// <summary>
        /// Creates a new entity and adds it.
        /// </summary>
        /// <returns>The newly created entity</returns>
        public static Entity CreateEntity()
        {
            Entity entity = EntityManager.Instantiate(s_EntityPrefab);
            s_Entities.Add(entity);

            return entity;
        }

        /// <summary>
        /// Creates an empty entity and adds it.
        /// </summary>
        /// <returns>The newly created entity</returns>
        public static Entity CreateEmptyEntity()
        {
            Entity entity = EntityManager.CreateEntity();
            s_Entities.Add(entity);

            return entity;
        }

        /// <summary>
        /// Destroys an entity and removes it.
        /// </summary>
        /// <param name="entity">The entity to remove</param>
        /// <returns>whether the operation was successful or not</returns>
        public bool DestroyEntity(Entity entity)
        {
            if (s_Entities.Contains(entity))
            {
                s_Entities.Remove(entity);
                EntityManager.DestroyEntity(entity);
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
