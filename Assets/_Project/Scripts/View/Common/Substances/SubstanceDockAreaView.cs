using OneMoreSpoon.Game.Systems;
using UnityEngine;

namespace OneMoreSpoon.View.Substances
{
    public sealed class SubstanceDockAreaView : MonoBehaviour
    {
        [SerializeField] private SubstanceDockKind dockKind;
        [SerializeField] private string label;
        [SerializeField] private Vector2 fallbackSize = new(4f, 2f);

        public SubstanceDockKind DockKind => dockKind;

        private void Awake()
        {
            if (TryGetComponent(out SpriteRenderer spriteRenderer))
                spriteRenderer.sortingOrder = -10;
        }

        public SubstanceDockArea ToArea()
        {
            return new SubstanceDockArea(
                dockKind,
                string.IsNullOrWhiteSpace(label) ? dockKind.ToString() : label,
                GetBounds());
        }

        private Bounds GetBounds()
        {
            if (TryGetComponent(out Collider2D collider))
                return collider.bounds;

            if (TryGetComponent(out SpriteRenderer spriteRenderer))
                return spriteRenderer.bounds;

            return new Bounds(transform.position, fallbackSize);
        }

        private void OnDrawGizmosSelected()
        {
            Bounds bounds = GetBounds();
            Gizmos.color = new Color(0.3f, 0.8f, 1f, 0.35f);
            Gizmos.DrawCube(bounds.center, bounds.size);
            Gizmos.color = new Color(0.1f, 0.4f, 0.8f, 0.9f);
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}
