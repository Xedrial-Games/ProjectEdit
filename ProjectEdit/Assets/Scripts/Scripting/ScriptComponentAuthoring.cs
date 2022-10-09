using UnityEngine;
using Unity.Entities;

namespace ProjectEdit.Scripting
{
    public class ScriptComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField] private string m_ScriptName;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem _)
            => dstManager.AddSharedComponentData(entity, new ScriptComponent { ScriptName = m_ScriptName });
    }
}
