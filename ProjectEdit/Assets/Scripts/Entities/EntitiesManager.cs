using UnityEngine;
using Unity.Entities;

using Xedrial.Physics;

namespace ProjectEdit.Entities
{
    public class EntitiesManager : MonoBehaviour
    {
        // The entity prefab game object.
        [SerializeField] private GameObject m_EntityGameObjectPrefab;

        public static EntityManager EntityManager { get; private set; }
        private World m_World;
        private BlobAssetStore m_BlobAssetStore;

        // The entity prefab created from the game object.
        private static Entity s_EntityPrefab;

        private void Awake()
        {
            // Get References
            m_World = World.DefaultGameObjectInjectionWorld;
            EntityManager = m_World.EntityManager;
            m_BlobAssetStore = new BlobAssetStore();

            GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(m_World, m_BlobAssetStore);

            // Ready the Entity prefab
            if (s_EntityPrefab == Entity.Null)
            { 
                s_EntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(m_EntityGameObjectPrefab, settings);
            }

            // Entity entity = CreateEntity();
            // EntityManager.AddComponentData(entity, new ScriptComponent { Script = "MoveSquare.lua" });
            //
            // Scripting.ScriptsSystem.AddScript(entity);
        }

        private void Start()
        {
            var physicsWorld = m_World.GetOrCreateSystem<PhysicsWorld>();
            physicsWorld.OnPhysicsWorldStart();
        }

        /// <summary>
        /// Creates an empty entity and adds it.
        /// </summary>
        /// <returns>The newly created entity</returns>
        public static Entity CreateEmptyEntity()
        {
            Entity entity = EntityManager.CreateEntity();
            EntityManager.AddComponent<RuntimeEntity>(entity);

            return entity;
        }

        /// <summary>
        /// Destroys an entity and removes it.
        /// </summary>
        /// <param name="entity">The entity to remove</param>
        /// <returns>whether the operation was successful or not</returns>
        public bool DestroyEntity(Entity entity)
        {
            if (EntityManager.Exists(entity))
            {
                EntityManager.DestroyEntity(entity);
                return true;
            }
            
            Debug.LogError("EntityCreator.DestroyEntity-> Invalid entity specified!", gameObject);
            return false;
        }

        private void OnDestroy()
        {
            m_BlobAssetStore.Dispose();
            if (!m_World.IsCreated)
            {
                s_EntityPrefab = Entity.Null;
                return;
            }
            s_EntityPrefab = Entity.Null;
            EntityManager.DestroyEntity(s_EntityPrefab);
        }
    }
}
