using OneMoreSpoon.View.Common;
using UnityEngine;

namespace OneMoreSpoon.View.Edges
{
    [RequireComponent(typeof(LineRenderer))]
    public sealed class EdgeView : EntityView
    {
        private LineRenderer lineRenderer;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;
        }

        private void LateUpdate()
        {
            if (World == null)
                return;

            if (!World.Edges.TryGetValue(EntityId, out var edge))
                return;

            if (!World.Positions.TryGetValue(edge.FromNodeId, out var fromPosition))
                return;

            if (!World.Positions.TryGetValue(edge.ToNodeId, out var toPosition))
                return;

            lineRenderer.SetPosition(0, fromPosition.Value);
            lineRenderer.SetPosition(1, toPosition.Value);
        }
    }
}