using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class EntityLink : MonoBehaviour, IConvertGameObjectToEntity
{
    public Entity Entity { get; set; }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        Entity = entity;
        dstManager.AddComponentObject(entity, this);
    }
}
