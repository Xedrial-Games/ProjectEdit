using System;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

using Timespawn.EntityTween.Tweens;

using b2Vec2 = System.Numerics.Vector2;

using Xedrial.Physics;

using ProjectEdit.Player;
using ProjectEdit.Components;
using Unity.Collections;

namespace ProjectEdit
{
    public struct MoveTriggerEvent : ITriggerEventsHandler
    {
        public void Execute(TriggerEvent triggerEvent, EntityManager entityManager)
        {
            if (triggerEvent.ContactState != ContactState.Enter)
                return;

            Xedrial.Physics.EntityPair entityPair = triggerEvent.EntityPair;
            bool isAPlayer = entityManager.HasComponent<PlayerTag>(entityPair.EntityA);
            bool isBPlayer = entityManager.HasComponent<PlayerTag>(entityPair.EntityB);
            
            bool isAMove = entityManager.HasComponent<MoveComponent>(entityPair.EntityA);
            bool isBMove = entityManager.HasComponent<MoveComponent>(entityPair.EntityB);

            bool ab = isAPlayer && isBMove;
            bool ba = isBPlayer && isAMove;
            if (!(ab || ba))
                return;

            MoveComponent moveComponent;
            Entity moveEntity;
            if (ab)
            {
                moveComponent = entityManager.GetComponentData<MoveComponent>(entityPair.EntityB);
                moveEntity = entityPair.EntityB;
            }
            else
            {
                moveComponent = entityManager.GetComponentData<MoveComponent>(entityPair.EntityA);
                moveEntity = entityPair.EntityA;
            }
            
            if (entityManager.HasComponent<TweenState>(moveEntity))
                return;
            
            Tween.Move(entityManager, moveEntity, moveComponent.Start, moveComponent.End, moveComponent.TweenParams);
        }
    }
    
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private GameObject m_PlayerContainer;

        private World m_World;
        private PhysicsWorld m_PhysicsWorld;

        private void Awake()
        {
            m_World = World.DefaultGameObjectInjectionWorld;
            m_PhysicsWorld = m_World.GetOrCreateSystem<PhysicsWorld>();
            
            Instantiate(m_PlayerContainer, new Vector3(0.5f, 0.5f, 0.0f), Quaternion.identity);
            m_PhysicsWorld.AddTriggerEventsHandler(new MoveTriggerEvent());
        }

        private void OnDestroy()
        {
            if (!m_World.IsCreated)
                return;
            
            var manager = m_World.GetExistingSystem<ManagerSystem>();
            manager?.OnStop();
        }
    }

    public partial class ManagerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAny<PlayerTag>().WithoutBurst().ForEach((Transform transform, ref Translation translation, ref Rotation rotation) =>
            {
                Vector3 position = transform.position;
                Quaternion rotationValue = transform.rotation;
                translation.Value = position;
                rotation.Value = rotationValue;
            }).Run();
        }

        public void OnStop()
        {
            EntityCommandBuffer ecb = new(Allocator.TempJob);
            Entities.WithAny<PlayerTag>().ForEach((Entity entity) =>
            {
                ecb.DestroyEntity(entity);
            }).Run();
            
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}
