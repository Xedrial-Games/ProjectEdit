using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

using Unity.Entities;

using TMPro;
using GamesTan.UI;
using DanielLochner.Assets.SimpleSideMenu;

using ProjectEdit.Tiles;
using ProjectEdit.ScriptableObjects;

namespace ProjectEdit.LevelsEditor.UI
{
    public class CellDataProvider : ISuperScrollRectDataProvider
    {
        public readonly List<CellData> CellsData = new();

        public int GetCellCount() => CellsData.Count;

        public void SetCell(GameObject cell, int index)
        {
            var item = cell.GetComponent<Cell>();
            item.BindData(CellsData[index]);
        }
    }
    
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        
        [SerializeField] private SuperScrollRect m_TilesScroll;
        [SerializeField] private SuperScrollRect m_EntitiesScroll;
        [SerializeField] private SimpleSideMenu m_InventoryMenu;

        [Space]

        [SerializeField] private Transform m_SelectCellsHolder;
        [SerializeField] private GameObject m_SelectCellPrefab;

        [Space]
        [SerializeField] private SimpleSideMenu m_EditMenu;
        [SerializeField] private TextMeshProUGUI m_XText;
        [SerializeField] private TextMeshProUGUI m_YText;

        [Space]
        [SerializeField] private GameObject m_PauseMenu;

        [Space]
        [SerializeField] private List<EntityPrefab> m_EntityPrefabs;

        private readonly CellDataProvider m_TilesCellDataProvider = new();
        private readonly CellDataProvider m_EntitiesCellDataProvider = new();

        private SelectCell[] m_SelectCells;

        private LevelLoader m_LevelLoader;
        private BlobAssetStore m_BlobAssetStore;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
            else Destroy(this);
            
            m_BlobAssetStore = new BlobAssetStore();

            InputSystem.Editor.Inventory.performed += _ => m_InventoryMenu.ToggleState();

            InputSystem.Editor.Esc.performed += _ => m_PauseMenu.SetActive(!m_PauseMenu.activeSelf);

            LevelEditor.OnEditorStateChanged += (prev, cur) =>
            {
                if (prev != EditorState.Edit && cur == EditorState.Edit)
                    m_EditMenu.Open();
                else if (prev == EditorState.Edit && cur != EditorState.Edit)
                    m_EditMenu.Close();
            };
         }

        private void Start()
        {
            m_LevelLoader = LevelLoader.Instance;
            
            List<Tile> tiles = TileCreator.Instance.Tiles;
            foreach (Tile t in tiles)
                m_TilesCellDataProvider.CellsData.Add(new CellData { Data = t, Sprite = t.sprite, Type = CellType.Tile});

            var world = World.DefaultGameObjectInjectionWorld;
            GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(world, m_BlobAssetStore);

            foreach (EntityPrefab e in m_EntityPrefabs)
            {
                Entity entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(e.GameObjectPrefab, settings);
                m_EntitiesCellDataProvider.CellsData.Add(new CellData
                {
                    Data = entity,
                    Sprite = e.InventoryIcon,
                    Type = CellType.Entity
                });
            }

            m_TilesScroll.DoAwake(m_TilesCellDataProvider);
            m_EntitiesScroll.DoAwake(m_EntitiesCellDataProvider);

            m_SelectCells = new SelectCell[9];
            for (int i = 0; i < 9; i++)
            {
                m_SelectCells[i] =
                    Instantiate(m_SelectCellPrefab, m_SelectCellsHolder).GetComponent<SelectCell>();

                m_SelectCells[i].Set(m_TilesCellDataProvider.CellsData[i]);
            }

            m_SelectCells[0].Select();
        }


        public void SaveLevel() => m_LevelLoader.SaveLevel();

        public void SaveAndQuit()
        {
            SaveLevel();
            SceneManager.LoadScene("MainMenu");
        }

        public void ClearSelectedTileProps()
        {
            m_XText.text = "X: ";
            m_YText.text = "Y: ";
        }

        public void SetSelectedTileProps(TileInstance tileData)
        {
            m_XText.text = $"X: {tileData.CellPosition.x}";
            m_YText.text = $"Y: {tileData.CellPosition.y}";
        }

        private void OnDestroy()
        {
            m_BlobAssetStore.Dispose();
        }
    }
}
