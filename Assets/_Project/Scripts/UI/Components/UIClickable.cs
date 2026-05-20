using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace FOIA.UI.Components
{
    [DisallowMultipleComponent]
    public sealed class UIClickable : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private bool interactable = true;
        [SerializeField] private PointerClickEvent clicked = new();

        public bool Interactable
        {
            get => interactable;
            set => interactable = value;
        }

        public UnityEvent<PointerEventData> Clicked => clicked;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!interactable || eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            clicked.Invoke(eventData);
        }

        [System.Serializable]
        private sealed class PointerClickEvent : UnityEvent<PointerEventData>
        {
        }
    }
}
