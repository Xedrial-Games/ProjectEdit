using ProjectEdit.Entities;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Entity = ProjectEdit.Entities.Entity;

namespace ProjectEdit
{
    public static class Physics
    {
        public static Entity RayCast(float3 from, float3 to)
        {
            var buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            CollisionWorld collisionWorld = buildPhysicsWorld.PhysicsWorld.CollisionWorld;

            RaycastInput raycastInput = new()
            {
                Start = from,
                End = to,
                Filter = new()
                {
                    BelongsTo = ~0u,
                    CollidesWith = ~0u,
                    GroupIndex = 0
                }
            };

            if (!collisionWorld.CastRay(raycastInput, out RaycastHit raycastHit))
                return new(Unity.Entities.Entity.Null, entityManager);
            
            Unity.Entities.Entity hitEntity = buildPhysicsWorld.PhysicsWorld.Bodies[raycastHit.RigidBodyIndex].Entity;
            return new(hitEntity, entityManager);
        }
    }
}
