using UnityEngine;
using GamesTan.UI;
using System.Collections.Generic;
using System;

namespace ProjectEdit.UI
{
    public class OnlineMenu : MonoBehaviour, ISuperScrollRectDataProvider
    {
        public static OnlineMenu Instance { get; private set; }

        [SerializeField] private SuperScrollRect m_OnlineLevelsRect;
        [SerializeField] private GameObject m_Loading;
        [SerializeField] private GameObject m_LevelMenu;

        private List<Level> m_Levels = new();

        private void Awake()
        {
            if (!Instance)
                Instance = this;
            else Destroy(this);

            m_OnlineLevelsRect.DoAwake(this);
            GameManager.Instance.OnlineSerializer.OnUpdateLevel += ReloadData;
        }

        public async void ReloadData()
        {
            if (!gameObject.activeSelf)
                return;

            m_OnlineLevelsRect.ReloadData();
            m_Loading.SetActive(true);
            try
            {
                m_Levels = await Database.GetLevels();
            }
            catch (Exception e)
            {
                Debug.LogError($"Exeption: {e}", gameObject);
            }
            m_Loading.SetActive(false);
            m_OnlineLevelsRect.ReloadData();
        }

        public void OnDisable() => m_Levels.Clear();

        public int GetCellCount() => m_Levels.Count;

        public void SetCell(GameObject cell, int index)
        {
            OnlineLevelCell levelCell = cell.GetComponent<OnlineLevelCell>();
            if (levelCell)
                levelCell.BindData(m_Levels[index], gameObject, m_LevelMenu);
        }
    }
}
