using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace ProjectEdit.UI
{
    [RequireComponent(typeof(Image))]
    public class TabButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [NonSerialized] public Image Background;

        [SerializeField] private TabGroup m_TabGroup;
        [SerializeField] private bool m_DefaultSelected = false;

        [SerializeField] private UnityEvent m_OnTabSelected;
        [SerializeField] private UnityEvent m_OnTabDeselected;
        
        private void Awake()
        {
            Background = GetComponent<Image>();
            m_TabGroup.Subscribe(this);
            if (m_DefaultSelected)
                m_TabGroup.OnTabSelected(this);
        }

        public void Select() => m_OnTabSelected?.Invoke();

        public void Deselect() => m_OnTabDeselected?.Invoke();

        public void OnPointerClick(PointerEventData eventData) => m_TabGroup.OnTabSelected(this);

        public void OnPointerEnter(PointerEventData eventData) => m_TabGroup.OnTabEnter(this);

        public void OnPointerExit(PointerEventData eventData) => m_TabGroup.OnTabExit(this);
    }
}
