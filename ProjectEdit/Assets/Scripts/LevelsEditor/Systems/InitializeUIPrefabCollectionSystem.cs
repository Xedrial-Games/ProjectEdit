using ProjectEdit.LevelsEditor.Components;
using ProjectEdit.LevelsEditor.UI;
using Unity.Entities;
using UnityEngine;

namespace ProjectEdit.LevelsEditor.Systems
{
    public partial class InitializeUIPrefabCollectionSystem : SystemBase
    {
        private class EntityCellDataProvider : IComponentData
        {
            public CellDataProvider Value;
        }

        private struct InitializeCellDataTag : IComponentData
        {
        }
        
        private UIManager m_UIManager;
        
        protected override void OnCreate()
        {
            m_UIManager = UIManager.Instance;
            EntityManager.AddComponentObject(SystemHandle, new EntityCellDataProvider
            {
                Value = new CellDataProvider()
            });

            EntityManager.CreateEntity(typeof(InitializeCellDataTag));
            
            RequireForUpdate<UIPrefabCollection>();
            RequireForUpdate<InitializeCellDataTag>();
        }
        
        protected override void OnUpdate()
        {
            EntityCellDataProvider cellData = EntityManager.GetComponentData<EntityCellDataProvider>(SystemHandle);
            
            if (!m_UIManager)
            {
                m_UIManager = UIManager.Instance;
                if (m_UIManager)
                    m_UIManager.DoAwakeEntityCellData(cellData.Value);
                else return;
            }

            DynamicBuffer<UIPrefabCollection> prefabs = SystemAPI.GetSingletonBuffer<UIPrefabCollection>();
            
            if (prefabs.Length == cellData.Value.CellsData.Count)
                return;

            foreach (UIPrefabCollection prefab in prefabs)
            {
                Sprite sprite = EntityManager.HasComponent<EntityIcon>(prefab.Prefab)
                    ? EntityManager.GetComponentData<EntityIcon>(prefab.Prefab).Value
                    : null;
                
                cellData.Value.CellsData.Add(new CellData
                {
                    Data = prefab.Prefab,
                    Sprite = sprite,
                    Type = CellType.Entity
                });
            }
            
            EntityManager.DestroyEntity(
                SystemAPI.GetSingletonEntity<InitializeCellDataTag>()
            );
        }
    }
}