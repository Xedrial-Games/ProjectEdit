using Unity.Mathematics;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

namespace ProjectEdit
{
    public static class Physics
    {
        public static Entity RayCast(float3 from, float3 to)
        {
            var buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
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
                return Entity.Null;

            return buildPhysicsWorld.PhysicsWorld.Bodies[raycastHit.RigidBodyIndex].Entity;
        }
    }
}
