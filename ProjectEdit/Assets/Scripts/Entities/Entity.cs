using Unity.Entities;
using UEntity = Unity.Entities.Entity;

namespace ProjectEdit.Entities
{
    public struct Entity
    {
        public Entity(UEntity entityHandle, EntityManager entityManager)
        {
            m_EntityHandle = entityHandle;
            m_EntityManager = entityManager;
        }

        /// <summary>
        /// Adds a component to the entity
        /// </summary>
        /// <typeparam name="T">the component type</typeparam>
        /// <returns>Whether the add was successful of not</returns>
        public bool AddComponent<T>() => m_EntityManager.AddComponent<T>(m_EntityHandle);

        /// <summary>
        /// Gets the specified component data
        /// </summary>
        /// <typeparam name="T">the component type</typeparam>
        /// <returns>the component data specified</returns>
        public T GetComponentData<T>() where T : struct, IComponentData
            => m_EntityManager.GetComponentData<T>(m_EntityHandle);

        /// <summary>
        /// Gets the specified component data
        /// </summary>
        /// <typeparam name="T">the component type</typeparam>
        /// <returns>the component data specified</returns>
        public T GetSharedComponentData<T>() where T : struct, ISharedComponentData
            => m_EntityManager.GetSharedComponentData<T>(m_EntityHandle);

        /// <summary>
        /// Set's the specified component data
        /// </summary>
        /// <param name="componentData">the value to set</param>
        /// <typeparam name="T">the component type</typeparam>
        public void SetComponentData<T>(T componentData) where T : struct, IComponentData
            => m_EntityManager.SetComponentData(m_EntityHandle, componentData);
        
        /// <summary>
        /// Checks whether the entity has a specific component
        /// </summary>
        /// <typeparam name="T">the component type</typeparam>
        /// <returns>true if the component exists, otherwise returns false</returns>
        public bool HasComponent<T>() where T : struct, IComponentData
            => m_EntityManager.HasComponent<T>(m_EntityHandle);

        public static implicit operator bool(Entity entity) => entity.m_EntityHandle != UEntity.Null;
        
        public static implicit operator UEntity(Entity entity) => entity.m_EntityHandle;

        public override string ToString()
        {
            return m_EntityHandle.ToString();
        }

        private readonly UEntity m_EntityHandle;
        private EntityManager m_EntityManager;
    }
}
