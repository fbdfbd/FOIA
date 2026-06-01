using UnityEngine;

namespace OneMoreSpoon.View.Common
{
    public sealed class SelectableOutlineVisual : MonoBehaviour
    {
        private static readonly int OutlineTintId = Shader.PropertyToID("_OutlineTint");

        [SerializeField] private SpriteRenderer targetRenderer;
        [SerializeField] private Color normalOutlineColor = Color.black;
        [SerializeField] private Color selectedOutlineColor = Color.yellow;

        private MaterialPropertyBlock propertyBlock;

        private void Awake()
        {
            EnsureRenderer();
            Apply(false);
        }

        private void Reset()
        {
            EnsureRenderer();
        }

        public void SetSelected(bool selected)
        {
            Apply(selected);
        }

        private void Apply(bool selected)
        {
            EnsureRenderer();

            if (targetRenderer == null)
                return;

            propertyBlock ??= new MaterialPropertyBlock();
            targetRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(OutlineTintId, selected ? selectedOutlineColor : normalOutlineColor);
            targetRenderer.SetPropertyBlock(propertyBlock);
        }

        private void EnsureRenderer()
        {
            if (targetRenderer == null)
                targetRenderer = GetComponent<SpriteRenderer>();
        }
    }
}
