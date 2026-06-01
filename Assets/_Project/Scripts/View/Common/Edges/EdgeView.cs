using OneMoreSpoon.View.Common;
using OneMoreSpoon.Game.Components;
using UnityEngine;

namespace OneMoreSpoon.View.Edges
{
    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(EdgeCollider2D))]
    public sealed class EdgeView : EntityView, ISelectableView
    {
        [Header("Visual")]
        [SerializeField] private Color forwardColor = Color.blue;
        [SerializeField] private Color reverseColor = Color.red;
        [SerializeField] private float normalWidth = 0.08f;
        [SerializeField] private float selectedWidth = 0.4f;

        [Header("Click")]
        [SerializeField] private float colliderRadius = 0.15f;

        [Header("Arrow")]
        [SerializeField] private Transform arrowHead;
        [SerializeField] private SpriteRenderer arrowRenderer;

        private LineRenderer lineRenderer;
        private EdgeCollider2D edgeCollider;
        private bool isSelected;
        private readonly Vector2[] colliderPoints = new Vector2[2];
        private bool hasRenderedEdge;
        private Vector3 lastFromPosition;
        private Vector3 lastToPosition;
        private OneMoreSpoon.Game.Core.EntityId lastFromNodeId;
        private OneMoreSpoon.Game.Core.EntityId lastToNodeId;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            edgeCollider = GetComponent<EdgeCollider2D>();

            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;

            edgeCollider.edgeRadius = colliderRadius;
            UpdateWidth();
        }

        public void RenderLine(EdgeComponent edge, Vector3 from, Vector3 to)
        {
            bool edgeMoved =
                !hasRenderedEdge ||
                lastFromPosition != from ||
                lastToPosition != to;

            if (edgeMoved)
            {
                lineRenderer.SetPosition(0, from);
                lineRenderer.SetPosition(1, to);

                UpdateCollider(from, to);
                UpdateArrow(from, to);

                lastFromPosition = from;
                lastToPosition = to;
            }

            bool routeChanged =
                !hasRenderedEdge ||
                lastFromNodeId != edge.FromNodeId ||
                lastToNodeId != edge.ToNodeId;

            if (routeChanged)
            {
                UpdateColor(edge.FromNodeId, edge.ToNodeId);
                lastFromNodeId = edge.FromNodeId;
                lastToNodeId = edge.ToNodeId;
            }

            hasRenderedEdge = true;
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;
            UpdateWidth();
        }

        private void UpdateColor(
            OneMoreSpoon.Game.Core.EntityId fromNodeId,
            OneMoreSpoon.Game.Core.EntityId toNodeId)
        {
            var color = IsForwardEdge(fromNodeId, toNodeId)
                ? forwardColor
                : reverseColor;

            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

            if (arrowRenderer != null)
                arrowRenderer.color = color;
        }

        private bool IsForwardEdge(
            OneMoreSpoon.Game.Core.EntityId fromNodeId,
            OneMoreSpoon.Game.Core.EntityId toNodeId)
        {
            if (!World.Nodes.TryGetValue(fromNodeId, out var fromNode))
                return false;

            if (!World.Nodes.TryGetValue(toNodeId, out var toNode))
                return false;

            return fromNode.ProcessLayer <= toNode.ProcessLayer;
        }

        private void UpdateWidth()
        {
            if (lineRenderer == null)
                return;

            lineRenderer.startWidth = isSelected ? selectedWidth : normalWidth;
            lineRenderer.endWidth = isSelected ? selectedWidth : normalWidth;
        }

        private void UpdateCollider(Vector3 from, Vector3 to)
        {
            var fromLocal = transform.InverseTransformPoint(from);
            var toLocal = transform.InverseTransformPoint(to);

            colliderPoints[0] = new Vector2(fromLocal.x, fromLocal.y);
            colliderPoints[1] = new Vector2(toLocal.x, toLocal.y);
            edgeCollider.points = colliderPoints;
        }

        private void UpdateArrow(Vector3 from, Vector3 to)
        {
            if (arrowHead == null)
                return;

            Vector3 direction = to - from;

            if (direction.sqrMagnitude <= 0.001f)
                return;

            arrowHead.position = Vector3.Lerp(from, to, 0.85f);

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            arrowHead.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
