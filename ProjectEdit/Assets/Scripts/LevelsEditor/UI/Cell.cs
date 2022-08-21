using GamesTan.UI;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectEdit.LevelsEditor.UI
{
    public struct CellData
    {
        public Sprite Sprite;
        public int Index;
    }

    public class Cell : MonoBehaviour, IScrollCell
    {
        [SerializeField] private Image m_Image;

        private CellData m_CellData;

        public void BindData(CellData cellData)
        {
            m_CellData = cellData;
            m_Image.sprite = cellData.Sprite;
            name = $"Cell_{cellData.Index}";
        }

        public void SetSelectCell() => SelectCell.SelectedCell.Set(m_CellData);
    }
}
