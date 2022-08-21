using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace ProjectEdit.Tiles
{
    public struct TileInstance
    {
        public Vector2Int CellPosition;
        public int Index;

        public TileInstance(Vector2Int cellPosition, int index)
        {
            CellPosition = cellPosition;
            Index = index;
        }

        public static implicit operator bool(TileInstance tile) => tile.Index != -1;
        public static implicit operator int(TileInstance tile) => tile.Index;
        public static implicit operator Vector3Int(TileInstance tile)
        {
            return (Vector3Int)tile.CellPosition;
        }
    }

    [DefaultExecutionOrder(-1)]
    public class TileCreator : MonoBehaviour
    {
        public static TileCreator Instance { get => s_Instance; }
        private static TileCreator s_Instance;

        public Dictionary<Vector3Int, TileInstance> TilesData { get { return m_TilesInstances; } }

        public List<Tile> Tiles { get => m_Tiles; }

        [SerializeField] private List<Tile> m_Tiles;

        private readonly Dictionary<Vector3Int, TileInstance> m_TilesInstances = new();

        private Tilemap m_Tilemap;
        private TileInstance m_SelectedTile;

        private void Awake()
        {
            if (!s_Instance)
                s_Instance = this;
            else Destroy(this);

            m_TilesInstances.Clear();
            m_SelectedTile.Index = -1;

            foreach (Tile tile in m_Tiles)
            {
                if (tile)
                    tile.color = Color.white;
            }
        }

        private void Start()
        {
            m_Tilemap = GetComponentInChildren<Tilemap>();
        }

        public TileInstance SelectTile(Vector3Int position)
        {
            ClearSelection();

            Tile tile = m_Tilemap.GetTile<Tile>(position);
            m_SelectedTile.Index = m_Tiles.FindIndex(a => a == tile);

            if (m_SelectedTile)
            {
                m_SelectedTile.CellPosition = (Vector2Int)position;
                m_Tiles[m_SelectedTile].color = Color.yellow;
                m_Tilemap.RefreshTile(position);
            }

            return m_SelectedTile;
        }

        public void ClearSelection()
        {
            if (m_SelectedTile)
            {
                m_Tiles[m_SelectedTile].color = Color.white;
                m_Tilemap.RefreshTile(m_SelectedTile);
                m_SelectedTile.Index = -1;
                m_SelectedTile.CellPosition = Vector2Int.zero;
            }
        }

        public void SetTile(Vector3Int position, int index)
        {
            if (index == 0)
                DeleteTile(position);

            Tile tile = m_Tiles[index];

            if (!m_Tilemap)
                m_Tilemap = GetComponentInChildren<Tilemap>();

            m_Tilemap.SetTile(position, tile);
            TileInstance tileInstance = new((Vector2Int)position, index);
            if (!m_TilesInstances.TryAdd(position, tileInstance))
                m_TilesInstances[position] = tileInstance;
        }

        public void DeleteTile(Vector3Int position)
        {
            if (!m_Tilemap)
                m_Tilemap = GetComponentInChildren<Tilemap>();

            m_Tilemap.SetTile(position, null);
            if (m_TilesInstances.ContainsKey(position))
                m_TilesInstances.Remove(position);
        }

        public void TileColor(Vector3Int position, Color color)
        {
            Tile tile = m_Tilemap.GetTile<Tile>(position);
            tile.color = color;
            m_Tilemap.RefreshTile(position);
        }

        public bool IsTile(Vector3Int position) => m_Tilemap.GetTile(position) != null;
    }
}
