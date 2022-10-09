using System;

using UnityEngine;
using UnityEngine.EventSystems;

using Unity.Mathematics;

using TMPro;

using ProjectEdit.Entities;
using ProjectEdit.LevelsEditor.UI;
using ProjectEdit.Tiles;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine.Tilemaps;

namespace ProjectEdit.LevelsEditor
{
    public enum EditorState
    {
        None, Brush, Delete, Edit
    }

    public static class Color32Extension
    {
        public static byte[] ToByteArray(this Color32 color) {
            return new[] {color.r, color.g, color.b, color.a};
        }
    }

    [DefaultExecutionOrder(-1)]
    public class LevelEditor : MonoBehaviour
    {
        public static LevelEditor Instance { get; private set; }

        public static event Action<EditorState, EditorState> OnEditorStateChanged;

        public CellData SelectedCell { get; set; }
        
        public Camera SelectionCamera => m_SelectionCamera;
        
        public Material SelectionMaterial => m_SelectionMaterial;

        [Header("UI, TODO: Move to UI Manager")]
        [SerializeField] private TextMeshProUGUI m_ToolText;
        
        [Header("Selection Properties")]
        [SerializeField] private Camera m_SelectionCamera;
        [SerializeField] private Material m_SelectionMaterial;
        
        private Vector3Int CellPosition
        {
            get
            {
                Vector3 worldPos = m_Camera.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int pos = m_Grid.WorldToCell(worldPos);
                pos.z = 0;
                return pos;
            }
        }
        
        private float3 CellWorldPosition => m_Grid.GetCellCenterWorld(CellPosition);

        private EditorState m_EditorState = EditorState.None;
        private bool m_IsLPressing;
        private bool m_IsAlt;
        private bool m_OverUI;

        private Camera m_Camera;
        private Grid m_Grid;
        private TileCreator m_TileCreator;
        
        private Texture2D m_SelectionTexture;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
            else Destroy(this);

            InputSystem.Init();

            InputSystem.Editor.Enable();

            // Left Mouse Button
            InputSystem.Editor.Perform.performed += _ =>
            {
                m_IsLPressing = true;

                if (m_IsAlt || m_OverUI)
                    return;

                switch (m_EditorState)
                {
                    case EditorState.Brush:
                    {
                        if (SelectedCell.Type == CellType.Entity)
                        {
                            Entity entity = EntitiesManager.EntityManager.Instantiate((Entity)SelectedCell.Data);
                            EntitiesManager.EntityManager.SetComponentData(entity, new Translation
                            {
                                Value = CellWorldPosition
                            });
                        }

                        break;
                    }
                    case EditorState.Edit:
                    {
                        if (!m_SelectionTexture)
                            m_SelectionTexture = SelectionCamera.targetTexture.ToTexture2D();
                        else SelectionCamera.targetTexture.ToTexture2D(m_SelectionTexture);

                        Vector2 pos = Input.mousePosition;
                        Color32 selectionColor = m_SelectionTexture.GetPixel((int)pos.x, (int)pos.y);
                        int index = BitConverter.ToInt32(selectionColor.ToByteArray());

                        if (index >= 1)
                        {
                            Entity selectedEntity = new()
                            {
                                Index = index,
                                Version = 1
                            };
                            print($"Index: {index}, Valid: {EntitiesManager.EntityManager.Exists(selectedEntity)}");
                        }

                        break;
                    }
                    case EditorState.Delete:
                        break;
                    case EditorState.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };

            InputSystem.Editor.Perform.canceled += _ => m_IsLPressing = false;

            // Alt Key
            InputSystem.Editor.Alt.performed += _ => m_IsAlt = true;
            InputSystem.Editor.Alt.canceled += _ => m_IsAlt = false;

            // E Key for Editing
            InputSystem.Editor.Edit.performed += _ =>
            {
                EditorState prev = m_EditorState;
                
                if (m_EditorState == EditorState.Edit)
                {
                    m_EditorState = EditorState.None;
                    m_ToolText.text = string.Empty;
                }
                else
                {
                    m_EditorState = EditorState.Edit;
                    m_ToolText.text = "Edit";
                }

                OnEditorStateChanged?.Invoke(prev, m_EditorState);
                m_TileCreator.ClearSelection();
            };

            // B Key for Brush
            InputSystem.Editor.Brush.performed += _ =>
            {
                EditorState prev = m_EditorState;
                
                switch (m_EditorState)
                {
                    case EditorState.Brush:
                        m_EditorState = EditorState.None;
                        m_ToolText.text = string.Empty;
                        
                        m_TileCreator.ClearSelection();
                        OnEditorStateChanged?.Invoke(prev, m_EditorState);
                        return;
                    case EditorState.Edit:
                        break;
                    case EditorState.None:
                        break;
                    case EditorState.Delete:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                m_EditorState = EditorState.Brush;
                m_ToolText.text = "Brush";

                m_TileCreator.ClearSelection();
                OnEditorStateChanged?.Invoke(prev, m_EditorState);
            };

            // D Key for Deleting
            InputSystem.Editor.Delete.performed += _ =>
            {
                EditorState prev = m_EditorState;
                
                switch (m_EditorState)
                {
                    case EditorState.Delete:
                        m_EditorState = EditorState.None;
                        m_ToolText.text = string.Empty;
                        
                        m_TileCreator.ClearSelection();
                        OnEditorStateChanged?.Invoke(prev, m_EditorState);
                        return;
                    case EditorState.Edit:
                        break;
                    case EditorState.None:
                        break;
                    case EditorState.Brush:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                m_EditorState = EditorState.Delete;
                m_ToolText.text = "Delete";

                m_TileCreator.ClearSelection();
                OnEditorStateChanged?.Invoke(prev, m_EditorState);
            };
            
            m_EditorState = EditorState.None;
            m_ToolText.text = string.Empty;
            
            m_Camera = Camera.main;

            m_TileCreator = GetComponent<TileCreator>();
            m_Grid = GetComponentInChildren<Grid>();
        }

        private void Start()
        {
            Debug.Assert(m_ToolText, "Tool text hasn't been assigned", gameObject);
            
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
            m_OverUI = EventSystem.current.IsPointerOverGameObject();
            
            if (m_OverUI || m_IsAlt || SelectedCell.Type != CellType.Tile)
                return;

            if (!m_IsLPressing)
                return;
            
            switch (m_EditorState)
            {
                case EditorState.Brush:
                    m_TileCreator.SetTile(CellPosition, (Tile)SelectedCell.Data);
                    break;
                case EditorState.Delete:
                    if (m_TileCreator.IsTile(CellPosition))
                        m_TileCreator.DeleteTile(CellPosition);
                    break;
                case EditorState.Edit:
                    m_TileCreator.SelectTile(CellPosition);
                    break;
                case EditorState.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public static class RenderTextureExtension
    {
        public static Texture2D ToTexture2D(this RenderTexture rt, Texture2D texture2D = null)
        {
            RenderTexture currentActiveRT = RenderTexture.active;
            
            // Set the supplied RenderTexture as the active one
            RenderTexture.active = rt;
 
            // Create a new Texture2D and read the RenderTexture image into it
            if (texture2D == null)
                texture2D = new Texture2D(rt.width, rt.height);
            texture2D.ReadPixels(new Rect(0, 0, texture2D.width, texture2D.height), 0, 0);
 
            // Restore previously active render texture
            RenderTexture.active = currentActiveRT;
            return texture2D;
        }
    }
}
