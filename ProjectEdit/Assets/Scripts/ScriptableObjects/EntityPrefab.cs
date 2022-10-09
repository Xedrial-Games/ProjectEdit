using UnityEngine;

namespace ProjectEdit.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewEntityPrefab", menuName = "Inventory/Entity Prefab", order = 0)]
    public class EntityPrefab : ScriptableObject
    {
        public GameObject GameObjectPrefab;
        public Sprite InventoryIcon;
    }
}