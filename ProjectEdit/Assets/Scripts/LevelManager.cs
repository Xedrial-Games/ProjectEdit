using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

using b2Vec2 = System.Numerics.Vector2;
using ProjectEdit.Player;
using Unity.Collections;
using Xedrial.Physics.b2D;

namespace ProjectEdit
{
    public struct MoveTriggerEvent : ITriggerEventsHandler
    {
        public void Execute(TriggerEvent triggerEvent, EntityManager entityManager)
        {
            if (triggerEvent.ContactState != ContactState.Enter)
                return;


            
            Debug.Log("Trigger");
        }
    }
    
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private GameObject m_PlayerContainer;

        private World m_World;

        private void Awake()
        {
            m_World = World.DefaultGameObjectInjectionWorld;
            
            Instantiate(m_PlayerContainer, new Vector3(0.5f, 0.5f, 0.0f), Quaternion.identity);
        }

        private void OnDestroy()
        {
            if (!m_World.IsCreated)
                return;
            
            var manager = m_World.GetExistingSystemManaged<ManagerSystem>();
            manager?.OnStop();
        }
    }

    public partial class ManagerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAny<PlayerTag>().WithoutBurst().ForEach((Transform transform, ref LocalTransform eTransform) =>
            {
                Vector3 position = transform.position;
                Quaternion rotationValue = transform.rotation;
                eTransform.Position = position;
                eTransform.Rotation = rotationValue;
            }).Run();
        }

        public void OnStop()
        {
            EntityCommandBuffer ecb = new(Allocator.TempJob);

            foreach ((_, Entity entity) in SystemAPI.Query<PlayerTag>().WithEntityAccess())
            {
                ecb.DestroyEntity(entity);
            }
            
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}
