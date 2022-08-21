using TMPro;
using Unity.Plastic.Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using ProjectEdit.Tiles;

namespace ProjectEdit.LevelsEditor
{
    public enum EditorState
    {
        Brush, Delete, Edit
    }

    [DefaultExecutionOrder(-1)]
    public class LevelEditor : MonoBehaviour
    {
        public static LevelEditor Instance { get; private set; }

        public static event Action OnEditStart;
        public static event Action OnEditEnd;

        public int SelectedTile { get; set; } = 1;

        [SerializeField] private TextMeshProUGUI m_ToolText;
        
        private Vector3Int CellPosition
        {
            get
            {
                Vector3 worldPos = m_MainCamera.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int pos = m_Grid.WorldToCell(worldPos);
                pos.z = 0;
                return pos;
            }
        }

        private EditorState m_EditorState = EditorState.Brush;
        private bool m_IsLPressing;
        private bool m_IsAlt;

        private Camera m_MainCamera;
        private Grid m_Grid;
        private TileCreator m_TileCreator;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
            else Destroy(this);

            InputSystem.Init();

            InputSystem.Editor.Enable();

            // Left Mouse Button
            InputSystem.Editor.LMB.performed += _ => m_IsLPressing = true;
            InputSystem.Editor.LMB.canceled += _ => m_IsLPressing = false;

            // Alt Key
            InputSystem.Editor.Alt.performed += _ => m_IsAlt = true;
            InputSystem.Editor.Alt.canceled += _ => m_IsAlt = false;

            // E Key for Editing
            InputSystem.Editor.Edit.performed += _ =>
            {
                if (m_EditorState == EditorState.Edit)
                    return;

                OnEditStart?.Invoke();

                m_EditorState = EditorState.Edit;
                m_ToolText.text = "Edit";

                m_TileCreator.ClearSelection();
            };

            // B Key for Brush
            InputSystem.Editor.Brush.performed += _ =>
            {
                switch (m_EditorState)
                {
                    case EditorState.Brush:
                        return;
                    case EditorState.Edit:
                        OnEditEnd?.Invoke();
                        break;
                }

                m_EditorState = EditorState.Brush;
                m_ToolText.text = "Brush";

                m_TileCreator.ClearSelection();
            };

            // D Key for Deleting
            InputSystem.Editor.Delete.performed += _ =>
            {
                switch (m_EditorState)
                {
                    case EditorState.Delete:
                        return;
                    case EditorState.Edit:
                        OnEditEnd?.Invoke();
                        break;
                }

                m_EditorState = EditorState.Delete;
                m_ToolText.text = "Delete";

                m_TileCreator.ClearSelection();
            };
        }

        private void Start()
        {
            Debug.Assert(m_ToolText, "Tool text hasn't been assigned", gameObject);

            m_MainCamera = Camera.main;

            m_TileCreator = GetComponent<TileCreator>();
            m_Grid = GetComponentInChildren<Grid>();

            //foreach (string path in Directory.GetFiles($"{Path}/Textures"))
            //{
            //    FileInfo fileInfo = new(path);
            //    if (fileInfo.Extension == ".png")
            //    {
            //        byte[] data = File.ReadAllBytes(path);
            //        Texture2D tex = new(0, 0);
            //        tex.filterMode = FilterMode.Point;

            //        if (tex.LoadImage(data))
            //        {
            //            GameObject go = Instantiate(m_SpriteInfo, m_SpritesHolder);
            //            go.GetComponentInChildren<Image>().sprite =
            //                Sprite.Create(tex, new(0, 0, tex.width, tex.height), new(0.5f, 0.5f), tex.width);
            //            go.GetComponentInChildren<TextMeshProUGUI>().text = fileInfo.Name;
            //        }
            //    }
            //}
        }

        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (m_IsLPressing && !m_IsAlt)
            {
                switch (m_EditorState)
                {
                    case EditorState.Brush:
                        m_TileCreator.SetTile(CellPosition, SelectedTile);
                        break;
                    case EditorState.Delete:
                        if (m_TileCreator.IsTile(CellPosition))
                            m_TileCreator.DeleteTile(CellPosition);
                        break;
                    case EditorState.Edit:
                        m_TileCreator.SelectTile(CellPosition);
                        break;
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
                SelectedTile = 1;
            if (Input.GetKeyDown(KeyCode.Alpha2))
                SelectedTile = 2;
        }
    }
}
