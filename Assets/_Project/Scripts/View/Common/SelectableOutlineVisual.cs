using UnityEngine;

namespace OneMoreSpoon.View.Common
{
    public sealed class SelectableOutlineVisual : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer targetRenderer;
        [SerializeField] private SpriteRenderer outlineRenderer;
        [SerializeField] private string outlineRendererName = "OutlineBackground";
        [SerializeField] private bool copyTargetSpriteWhenEmpty = true;
        [SerializeField] private Color normalOutlineColor = Color.black;
        [SerializeField] private Color selectedOutlineColor = Color.yellow;

        private bool isSelected;

        private void Awake()
        {
            EnsureRenderers();
            Apply();
        }

        private void Reset()
        {
            EnsureRenderers();
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;
            Apply();
        }

        public void SetColors(Color normal, Color selected)
        {
            normalOutlineColor = normal;
            selectedOutlineColor = selected;
            Apply();
        }

        private void Apply()
        {
            EnsureRenderers();

            if (outlineRenderer == null)
                return;

            if (copyTargetSpriteWhenEmpty
                && outlineRenderer.sprite == null
                && targetRenderer != null)
            {
                outlineRenderer.sprite = targetRenderer.sprite;
            }

            outlineRenderer.color = isSelected ? selectedOutlineColor : normalOutlineColor;
        }

        private void EnsureRenderers()
        {
            if (targetRenderer == null)
                targetRenderer = GetComponent<SpriteRenderer>();

            if (outlineRenderer == null)
                outlineRenderer = FindOutlineRenderer();
        }

        private SpriteRenderer FindOutlineRenderer()
        {
            Transform outlineTransform = FindChildRecursive(transform, outlineRendererName);

            return outlineTransform != null
                ? outlineTransform.GetComponent<SpriteRenderer>()
                : null;
        }

        private static Transform FindChildRecursive(Transform parent, string childName)
        {
            if (parent == null || string.IsNullOrEmpty(childName))
                return null;

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);

                if (child.name == childName)
                    return child;

                Transform match = FindChildRecursive(child, childName);
                if (match != null)
                    return match;
            }

            return null;
        }
    }
}
