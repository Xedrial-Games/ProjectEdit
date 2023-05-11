using UnityEngine;
using Unity.Entities;

namespace ProjectEdit.Entities
{
    public class EntitiesManager : MonoBehaviour
    {
        public static EntityManager EntityManager { get; private set; }
        private World m_World;

        private void Awake()
        {
            // Get References
            m_World = World.DefaultGameObjectInjectionWorld;
            EntityManager = m_World.EntityManager;

            // Entity entity = CreateEntity();
            // EntityManager.AddComponentData(entity, new ScriptComponent { Script = "MoveSquare.lua" });
            //
            // Scripting.ScriptsSystem.AddScript(entity);
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
    }
}
