using OneMoreSpoon.View.Common;
using UnityEngine;
using TMPro;

namespace OneMoreSpoon.View.Nodes
{
    public sealed class NodeView : EntityView, ISelectableView
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color selectedColor = Color.yellow;

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            if (label == null)
                label = GetComponentInChildren<TMP_Text>();
        }

        private void LateUpdate()
        {
            if (World == null)
                return;

            if (!World.Positions.TryGetValue(EntityId, out var position))
                return;

            transform.position = position.Value;
        }

        public void SetSelected(bool selected)
        {
            if (spriteRenderer == null)
                return;

            spriteRenderer.color = selected ? selectedColor : normalColor;
        }

        public void SetLabel(string text)
        {
            if (label == null)
                return;
            label.text = text;
        }
    }
}
