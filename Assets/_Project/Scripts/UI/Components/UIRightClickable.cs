using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace FOIA.UI.Components
{
    [DisallowMultipleComponent]
    public sealed class UIRightClickable : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private bool interactable = true;
        [SerializeField] private PointerClickEvent rightClicked = new();

        public bool Interactable
        {
            get => interactable;
            set => interactable = value;
        }

        public UnityEvent<PointerEventData> RightClicked => rightClicked;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!interactable || eventData.button != PointerEventData.InputButton.Right)
            {
                return;
            }

            rightClicked.Invoke(eventData);
        }

        [System.Serializable]
        private sealed class PointerClickEvent : UnityEvent<PointerEventData>
        {
        }
    }
}
