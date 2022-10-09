using UnityEngine;
using Unity.Entities;

namespace ProjectEdit
{
    public class LogTrigger : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField] private string m_Message;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem _)
        {
            dstManager.AddComponentObject(entity, this);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            print(m_Message);
        }
    }
}
