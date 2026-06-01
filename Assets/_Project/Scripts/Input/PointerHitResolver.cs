using OneMoreSpoon.View.Common;
using OneMoreSpoon.View.Edges;
using OneMoreSpoon.View.Nodes;
using OneMoreSpoon.View.Substances;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace OneMoreSpoon.Input
{
    public sealed class PointerHitResolver : MonoBehaviour
    {
        [SerializeField] private LayerMask nodeLayer;
        [SerializeField] private LayerMask edgeLayer;

        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        public PointerHit Resolve()
        {
            if (Pointer.current == null)
                return PointerHit.None();

            if (IsPointerOverUI())
                return PointerHit.UI();

            Vector2 worldPosition = GetWorldPosition();
            RaycastHit2D[] hits = Physics2D.RaycastAll(worldPosition, Vector2.zero);

            if (ContainsMergeSlotHandle(hits))
                return PointerHit.MergeSlotHandle();

            NodeView nodeView = RaycastNodeView(worldPosition);
            if (nodeView != null)
                return PointerHit.Node(nodeView);

            SubstanceView substanceView = FindSubstanceView(hits);
            if (substanceView != null)
                return PointerHit.Substance(substanceView);

            EdgeView edgeView = RaycastEdgeView(worldPosition);
            if (edgeView != null)
                return PointerHit.Edge(edgeView);

            return PointerHit.None();
        }

        public Vector2 GetWorldPosition()
        {
            EnsureCamera();
            Vector2 screenPosition = Pointer.current.position.ReadValue();
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
            return new Vector2(worldPosition.x, worldPosition.y);
        }

        public bool IsPointerOverTrashCan()
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(GetWorldPosition(), Vector2.zero);

            foreach (var hit in hits)
            {
                if (hit.collider.GetComponentInParent<TrashCanView>() != null)
                    return true;
            }

            return false;
        }

        private void EnsureCamera()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
        }

        private bool IsPointerOverUI()
        {
            return EventSystem.current != null
                && EventSystem.current.IsPointerOverGameObject();
        }

        private static bool ContainsMergeSlotHandle(RaycastHit2D[] hits)
        {
            foreach (var hit in hits)
            {
                if (hit.collider.GetComponentInParent<MergeSlotHandle>() != null)
                    return true;
            }

            return false;
        }

        private NodeView RaycastNodeView(Vector2 worldPosition)
        {
            var hit = Physics2D.Raycast(worldPosition, Vector2.zero, Mathf.Infinity, nodeLayer);
            return hit.collider == null
                ? null
                : hit.collider.GetComponentInParent<NodeView>();
        }

        private static SubstanceView FindSubstanceView(RaycastHit2D[] hits)
        {
            foreach (var hit in hits)
            {
                var substanceView = hit.collider.GetComponentInParent<SubstanceView>();
                if (substanceView != null)
                    return substanceView;
            }

            return null;
        }

        private EdgeView RaycastEdgeView(Vector2 worldPosition)
        {
            var hit = Physics2D.Raycast(worldPosition, Vector2.zero, Mathf.Infinity, edgeLayer);
            return hit.collider == null
                ? null
                : hit.collider.GetComponentInParent<EdgeView>();
        }
    }
}
