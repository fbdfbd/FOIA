using OneMoreSpoon.View.Substances;
using UnityEngine;

namespace OneMoreSpoon.Game.Systems
{
    public interface ISubstanceCardMetricsProvider
    {
        Vector2 CardSize { get; }
    }

    public sealed class SubstanceCardMetricsProvider : ISubstanceCardMetricsProvider
    {
        private static readonly Vector2 FallbackCardSize = new(3.6f, 4.3f);

        public Vector2 CardSize { get; }

        public SubstanceCardMetricsProvider(SubstanceView substanceViewPrefab)
        {
            CardSize = substanceViewPrefab != null &&
                substanceViewPrefab.TryGetComponent(out BoxCollider2D collider)
                    ? collider.size
                    : FallbackCardSize;
        }
    }
}
