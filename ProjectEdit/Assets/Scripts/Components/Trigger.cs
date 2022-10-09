using System;

using UnityEngine;

using Unity.Entities;

using ProjectEdit.Components;
using Timespawn.EntityTween.Tweens;
using Unity.Transforms;

namespace ProjectEdit
{
    public struct EntityPair
    {
        public Entity EntityA;
        public Entity EntityB;

        public EntityPair(Entity entityA, Entity entityB)
        {
            EntityA = entityA;
            EntityB = entityB;
        }
    }

    namespace Components
    {
        [RequireComponent(typeof(EntityLink))]
        public class Trigger : MonoBehaviour, IConvertGameObjectToEntity
        {
            public bool Enter;
            public bool Exit;
            public bool Stay;

            private EntityLink m_EntityLink;

            private void Awake() => m_EntityLink = GetComponent<EntityLink>();

            public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem _)
                => dstManager.AddComponentObject(entity, this);

            public void OnTriggerEnter2D(Collider2D other)
            {
                Enter = true;
            }

            public void OnTriggerExit2D(Collider2D other)
            {
                Exit = true;
            }

            public void OnTriggerStay2D(Collider2D other)
            {
                Stay = true;
            }
        }
    }

    public partial class TriggerSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Trigger trigger, ref LogComponent log) =>
            {
                if (trigger.Enter)
                    log.Log();
            });

            Entities.ForEach((Entity entity, Trigger trigger, ref Translation translation, ref MoveComponent move) =>
            {
                if (trigger.Enter)
                    Tween.Move(EntityManager, entity, translation.Value, move.End, move.TweenParams);
            });

            Entities.ForEach((Trigger trigger) =>
            {
                if (trigger.Enter)
                    trigger.Enter = false;

                if (trigger.Exit)
                    trigger.Exit = false;
            });
        }
    }
}
