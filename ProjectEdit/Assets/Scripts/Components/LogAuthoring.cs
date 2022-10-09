using ProjectEdit.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Trigger))]
[DisallowMultipleComponent]
public class LogAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] private string m_Message;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem _)
    {
        dstManager.AddComponentData(entity, new LogComponent { Message = m_Message });
    }
}
