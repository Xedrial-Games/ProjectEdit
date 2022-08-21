using UnityEngine;
using GamesTan.UI;

namespace ProjectEdit.UI
{
    public class SavedLevelsMenu : MonoBehaviour, ISuperScrollRectDataProvider
    {
        public static SavedLevelsMenu Instance { get; private set; }

        [SerializeField] private SuperScrollRect m_SavedLevels;
        [SerializeField] private GameObject m_LevelMenu;

        private ref Serializer Serializer => ref GameManager.Instance.OnlineSerializer;

        private SavedLevelsMenu() => Instance = this;

        private void Awake()
        {
            m_SavedLevels.DoAwake(this);
            Serializer.OnUpdateLevel += ReloadData;
        }

        private void OnDestroy() => Serializer.OnUpdateLevel -= ReloadData;

        public void ReloadData()
        {
            if (gameObject.activeSelf)
                m_SavedLevels.ReloadData();
        }

        public int GetCellCount() => Serializer.Levels.Count;

        public void SetCell(GameObject cell, int index)
        {
            OnlineLevelCell levelCell = cell.GetComponent<OnlineLevelCell>();
            if (levelCell)
                levelCell.BindData(Serializer.Levels[index], gameObject, m_LevelMenu);
        }
    }
}
