using UnityEngine;
using Unity.Entities;
using Xedrial.Physics;
using b2Vec2 = System.Numerics.Vector2;

namespace ProjectEdit.Player
{
    public partial class FollowTransformSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAny<PlayerTag>().WithoutBurst().ForEach((Transform transform, in RigidBody2DComponent rb) =>
            {
                if (rb.Body == null)
                    return;
                
                Vector3 tPos = transform.position;
                b2Vec2 position = new(tPos.x, tPos.y);
                rb.Body.SetTransform(position, rb.Body.GetAngle());
            }).Run();
        }
    }
}