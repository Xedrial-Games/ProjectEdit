using System.Collections.Generic;
using UnityEngine;

namespace ProjectEdit
{
    public class GridGFX : MonoBehaviour
    {
		[SerializeField] private GameObject m_GridGFX;
        [SerializeField] private float m_Offset = 2f;

        private Vector3Int CellPosition(Vector3 position) => m_Grid.WorldToCell(position);
        private Vector3 WorldPosition(Vector3Int cellPosition) => m_Grid.GetCellCenterWorld(cellPosition);

        private Vector2 CameraExtend
        {
            get
            {
                Vector2 result;
                result.x = m_Camera.aspect * m_Camera.orthographicSize;
                result.y = m_Camera.aspect * m_Camera.orthographicSize;

                return result;
            }
        }

        private readonly List<Vector3Int> m_GridPositions = new();

        private Camera m_Camera;
        private Grid m_Grid;
        private Transform m_CamTransform;

        private void Start()
        {
            m_Camera = Camera.main;
            m_CamTransform = m_Camera.transform;
            m_Grid = GetComponent<Grid>();
        }

        private void Update()
        {
            InstantiateGrid(new(m_CamTransform.position.x - CameraExtend.x - m_Offset, m_Camera.transform.position.y));
            InstantiateGrid(new(m_CamTransform.position.x + CameraExtend.x + m_Offset, m_Camera.transform.position.y));
            InstantiateGrid(new(m_CamTransform.position.x, m_Camera.transform.position.y - CameraExtend.y - m_Offset));
            InstantiateGrid(new(m_CamTransform.position.x, m_Camera.transform.position.y + CameraExtend.y + m_Offset));
            InstantiateGrid(new(m_CamTransform.position.x - CameraExtend.x - m_Offset, m_Camera.transform.position.y - CameraExtend.y - m_Offset));
            InstantiateGrid(new(m_CamTransform.position.x + CameraExtend.x + m_Offset, m_Camera.transform.position.y + CameraExtend.y + m_Offset));
            InstantiateGrid(new(m_CamTransform.position.x + CameraExtend.x + m_Offset, m_Camera.transform.position.y - CameraExtend.y - m_Offset));
            InstantiateGrid(new(m_CamTransform.position.x - CameraExtend.x - m_Offset, m_Camera.transform.position.y + CameraExtend.y + m_Offset));
        }

        private void InstantiateGrid(Vector2 worldPosition)
        {
            Vector3Int cellPosition = CellPosition(worldPosition);
            if (!m_GridPositions.Contains(cellPosition))
            {
                Instantiate(m_GridGFX, WorldPosition(cellPosition), Quaternion.Euler(0.0f, 0.0f, 90.0f), transform);
                m_GridPositions.Add(cellPosition);
            }
        }
    }
}
