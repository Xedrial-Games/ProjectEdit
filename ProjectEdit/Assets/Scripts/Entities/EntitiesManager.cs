using System.Collections.Generic;
using ProjectEdit.Components;
using ProjectEdit.Scripting;
using UnityEngine;

using Unity.Entities;
using UEntity = Unity.Entities.Entity;

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
            m_BlobAssetStore = new BlobAssetStore();

            // Ready the entities list
            m_EntityInstances.Clear();
            
            // Ready the Entity prefab
            GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(m_World, m_BlobAssetStore);
            m_EntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy
                (m_EntityGameObjectPrefab, settings);

            Entity entity = CreateEntity();
            if (entity.AddComponent<ScriptComponent>())
                entity.SetComponentData(new ScriptComponent{ Name = "Test.lua" });
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

    public partial class ScriptSystem : SystemBase
    {
        protected override void OnCreate()
        {
            ScriptEngine.LoadLua();
        }

        protected override void OnStartRunning()
        {
            var scripts = ScriptEngine.Scripts;

            Entities.ForEach((ref ScriptComponent scriptComponent) =>
            {
                if (!scripts.TryGetValue(scriptComponent.Name.ToString(), out Script script))
                    return;

                script.StartFunction?.Call();
            }).WithoutBurst().Run();
        }

        protected override void OnUpdate()
        {
            var scripts = ScriptEngine.Scripts;

            Entities.ForEach((ref ScriptComponent scriptComponent) =>
            {
                if (!scripts.TryGetValue(scriptComponent.Name.ToString(), out Script script))
                    return;

                script.UpdateFunction?.Call();
            }).WithoutBurst().Run();
        }
    }
}
