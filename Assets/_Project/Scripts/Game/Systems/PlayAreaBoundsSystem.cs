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
            Bounds bounds = GetInnerBounds();

            float minX = bounds.min.x;
            float maxX = bounds.max.x;
            float minY = bounds.min.y;
            float maxY = bounds.max.y;

            return new Vector2(Mathf.Clamp(position.x, minX, maxX), Mathf.Clamp(position.y, minY, maxY));
        }

        public Bounds GetInnerBounds()
        {
            Bounds bounds = panelRenderer.bounds;
            bounds.Expand(-padding * 2f);
            return bounds;
        }

        public bool Contains(Vector2 position)
        {
            Vector2 clampedPosition = Clamp(position);
            return clampedPosition == position;
        }
    }
}
