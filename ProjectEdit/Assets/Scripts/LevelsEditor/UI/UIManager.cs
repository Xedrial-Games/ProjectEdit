using System.Collections.Generic;
using DanielLochner.Assets.SimpleSideMenu;
using GamesTan.UI;
using ProjectEdit.Tiles;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace ProjectEdit.LevelsEditor.UI
{
    public class UIManager : MonoBehaviour, ISuperScrollRectDataProvider
    {
        public static UIManager Instance => s_Instance;
        private static UIManager s_Instance;

        public SelectCell[] SelectCells => m_SelectCells;

        [SerializeField] private SuperScrollRect m_InventoryScroll;
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

        private SelectCell[] m_SelectCells;
        private readonly List<CellData> m_CellsData = new();

        private LevelLoader m_LevelLoader;

        private void Awake()
        {
            if (!s_Instance)
                s_Instance = this;
            else Destroy(this);

            m_CellsData.Clear();

            InputSystem.Editor.Inventory.performed += _ =>
            {
                m_InventoryMenu.ToggleState();
            };

            InputSystem.Editor.Esc.performed += _ =>
            {
                m_PauseMenu.SetActive(!m_PauseMenu.activeSelf);
            };

            LevelEditor.OnEditStart += m_EditMenu.Open;
            LevelEditor.OnEditEnd += m_EditMenu.Close;
         }

        private void Start()
        {
            m_LevelLoader = LevelLoader.Instance;

            List<Tile> tiles = TileCreator.Instance.Tiles;
            for (int i = 0; i < tiles.Count; i++)
                m_CellsData.Add(new CellData { Index = i, Sprite = tiles[i].sprite });

            m_InventoryScroll.DoAwake(this);

            m_SelectCells = new SelectCell[9];
            for (int i = 0; i < 9; i++)
            {
                m_SelectCells[i] =
                    Instantiate(m_SelectCellPrefab, m_SelectCellsHolder).GetComponent<SelectCell>();

                m_SelectCells[i].Set(m_CellsData[i]);
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

        public int GetCellCount() => m_CellsData.Count;

        public void SetCell(GameObject cell, int index)
        {
            Cell item = cell.GetComponent<Cell>();
            item.BindData(m_CellsData[index]);
        }
    }
}
