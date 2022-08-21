using Unity.Entities;
using Unity.Burst;

namespace ProjectEdit.Entities
{
    public struct EntityInstance : IComponentData
    {
        public Entity Entity;
        public float Time;

        private EntityManager m_EntityManager;

        public EntityInstance(Entity entity, EntityManager entityManager)
        {
            Entity = entity;
            m_EntityManager = entityManager;
            Time = 0.0f;
        }

        /// <summary>
        /// Gets the specified component data
        /// </summary>
        /// <typeparam name="T">the component type</typeparam>
        /// <returns>the component data specified</returns>
        public T GetComponentData<T>() where T : struct, IComponentData
            => m_EntityManager.GetComponentData<T>(Entity);

        /// <summary>
        /// Set's the specified component data
        /// </summary>
        /// <param name="componentData">the value to set</param>
        /// <typeparam name="T">the component type</typeparam>
        public void SetComponentData<T>(T componentData) where T : struct, IComponentData
            => m_EntityManager.SetComponentData(Entity, componentData);
        
        /// <summary>
        /// Checks whether the entity has a specific component
        /// </summary>
        /// <typeparam name="T">the component type</typeparam>
        /// <returns>true if the component exists, otherwise returns false</returns>
        public bool HasComponent<T>() where T : struct, IComponentData
            => m_EntityManager.HasComponent<T>(Entity);
    }

    public partial class EntityInstancesSystem : SystemBase
    {
        private EntityQuery m_Query;

        protected override void OnCreate()
        {
            // Cached access to a set of ComponentData based on a specific query
            m_Query = GetEntityQuery(typeof(EntityInstance));
        }

        // Use the [BurstCompile] attribute to compile a job with Burst. You may see significant speed ups, so try it!
        [BurstCompile]
        public struct EntityInstancesJob : IJobEntityBatch
        {
            public float DeltaTime;
            public ComponentTypeHandle<EntityInstance> CreatorEntityTypeHandle;

            public void Execute(ArchetypeChunk batchInChunk, int batchIndex)
            {
                var chunkCreatorEntities = batchInChunk.GetNativeArray(CreatorEntityTypeHandle);
                for (var i = 0; i < batchInChunk.Count; i++)
                {
                    EntityInstance creatorEntity = chunkCreatorEntities[i];

                    // Rotate something about its up vector at the speed given by RotationSpeed_IJobChunk.
                    chunkCreatorEntities[i] = new EntityInstance
                    {
                        Time = creatorEntity.Time + DeltaTime
                    };
                }
            }
        }

        // OnUpdate runs on the main thread.
        protected override void OnUpdate()
        {
            // Explicitly declare:
            // - Read-Write access to CreatorEntity
            var rotationSpeedType = GetComponentTypeHandle<EntityInstance>();

            var job = new EntityInstancesJob()
            {
                CreatorEntityTypeHandle = rotationSpeedType,
                DeltaTime = Time.DeltaTime
            };

            Dependency = job.ScheduleParallel(m_Query, Dependency);
        }
    }
}
