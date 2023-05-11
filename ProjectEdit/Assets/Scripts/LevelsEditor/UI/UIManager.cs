using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

using Unity.Entities;

using TMPro;
using GamesTan.UI;
using DanielLochner.Assets.SimpleSideMenu;

using ProjectEdit.UI;
using ProjectEdit.Tiles;
using Unity.Collections;

using Unity.Transforms;

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
        [SerializeField] private UIDocument m_InspectorPanelDoc;
        [SerializeField] private VisualTreeAsset m_TabButton;

        private readonly CellDataProvider m_TilesCellDataProvider = new();

        private SelectCell[] m_SelectCells;

        private InputActions m_Input;
        private LevelLoader m_LevelLoader;

        // INSPECTOR PANEL
        private InspectorPanel m_InspectorPanel;

        private ListView m_ComponentsView;
        private Label m_ComponentName;
        private List<ComponentType> m_ComponentTypes;
        
        private Entity m_SelectedEntity;
        private World m_World;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
            else Destroy(this);
            
            RegisterInput();

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

            m_World = World.DefaultGameObjectInjectionWorld;

            m_TilesScroll.DoAwake(m_TilesCellDataProvider);

            m_SelectCells = new SelectCell[9];
            for (int i = 0; i < 9; i++)
            {
                m_SelectCells[i] =
                    Instantiate(m_SelectCellPrefab, m_SelectCellsHolder).GetComponent<SelectCell>();

                m_SelectCells[i].Set(m_TilesCellDataProvider.CellsData[i]);
            }

            m_SelectCells[0].Select();

            m_ComponentsView = m_InspectorPanelDoc.rootVisualElement?.Q<ListView>("components-view");
            m_ComponentName = m_InspectorPanelDoc.rootVisualElement?.Q<Label>("component-name");
        }

        public void DoAwakeEntityCellData(ISuperScrollRectDataProvider d) => m_EntitiesScroll.DoAwake(d);

        public void SetInspectedEntity(Entity entity)
        {
            m_SelectedEntity = entity;

            NativeArray<ComponentType> componentTypes = m_World.EntityManager.GetComponentTypes(m_SelectedEntity);
            m_ComponentTypes = componentTypes.ToArray().ToList();
            componentTypes.Dispose();
            
            m_ComponentsView.makeItem += () =>
            {
                TemplateContainer newListEntry = m_TabButton.Instantiate();

                var listEntryLogic = new ComponentListEntryController();
                listEntryLogic.Initialize(newListEntry);

                newListEntry.userData = listEntryLogic;

                return newListEntry;
            };

            m_ComponentsView.bindItem = (element, i) =>
            {
                (element.userData as ComponentListEntryController)?.SetComponentData(m_ComponentTypes[i]);
            };

            m_ComponentsView.itemsSource = m_ComponentTypes;

            m_ComponentsView.selectionChanged += _ =>
            {
                var selectedComponent = (ComponentType)m_ComponentsView.selectedItem;

                if (selectedComponent.Equals(typeof(LocalTransform)))
                {
                    var localTransform = m_World.EntityManager.GetComponentData<LocalTransform>(entity);
                    m_ComponentName.text = "Local Transform";
                }
            };
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

        private void RegisterInput()
        {
            m_Input = new InputActions();
            m_Input.Enable();
            
            m_Input.Editor.Inventory.performed += _ => m_InventoryMenu.ToggleState();
            m_Input.Editor.Esc.performed += _ => m_PauseMenu.SetActive(!m_PauseMenu.activeSelf);
        }
    }
}
