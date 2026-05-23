using UnityEngine;

namespace OneMoreSpoon.Game.Systems
{
    public sealed class PlayAreaBoundsSystem
    {
        private SpriteRenderer panelRenderer;
        private float padding;

        public PlayAreaBoundsSystem(SpriteRenderer panelRenderer, float padding)
        {
            this.panelRenderer = panelRenderer;
            this.padding = padding;
        }

        public Vector2 Clamp(Vector2 position)
        {
            Bounds bounds = panelRenderer.bounds;

            float minX = bounds.min.x + padding;
            float maxX = bounds.max.x - padding;
            float minY = bounds.min.y + padding;
            float maxY = bounds.max.y - padding;

            return new Vector2(Mathf.Clamp(position.x, minX, maxX), Mathf.Clamp(position.y, minY, maxY));
        }

        public bool Contains(Vector2 position)
        {
            Vector2 clampedPosition = Clamp(position);
            return clampedPosition == position;
        }
    }
}
