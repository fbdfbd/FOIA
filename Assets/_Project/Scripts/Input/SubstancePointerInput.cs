using OneMoreSpoon.App.State;
using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Definitions;
using OneMoreSpoon.Game.Systems;
using OneMoreSpoon.View.Common;
using OneMoreSpoon.View.Edges;
using OneMoreSpoon.View.Nodes;
using OneMoreSpoon.View.Substances;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using VContainer;

namespace OneMoreSpoon.Input
{
    public sealed class SubstancePointerInput : MonoBehaviour
    {
        [SerializeField] private LayerMask substanceLayer;
        [SerializeField] private float dragStartThreshold = 0.12f;

        private GameWorld world;
        private SubstanceStackSystem stackSystem;
        private MergeSystem mergeSystem;
        private EdgeBlockEquipSystem edgeBlockEquipSystem;
        private ClusterSeparationSystem clusterSeparationSystem;
        private SubstanceDockSystem substanceDockSystem;
        private SubstanceDefinitionRegistry substanceDefinitionRegistry;
        private ViewRegistry viewRegistry;
        private SelectionVisualService selectionVisualService;

        private Camera mainCamera;
        private SubstanceView pendingView;
        private SubstanceView draggingView;
        private SubstanceDockDragOrigin dragOrigin;
        private Vector2 pointerDownPosition;
        private Vector2 dragStartPosition;
        private Vector2 pointerToViewOffset;

        private enum DropResult
        {
            Free,
            Docked,
            Accepted,
            ReturnToOrigin,
            Removed
        }

        [Inject]
        public void Construct(
            GameWorld world,
            SubstanceStackSystem stackSystem,
            MergeSystem mergeSystem,
            EdgeBlockEquipSystem edgeBlockEquipSystem,
            ClusterSeparationSystem clusterSeparationSystem,
            SubstanceDockSystem substanceDockSystem,
            SubstanceDefinitionRegistry substanceDefinitionRegistry,
            ViewRegistry viewRegistry,
            SelectionVisualService selectionVisualService)
        {
            this.world = world;
            this.stackSystem = stackSystem;
            this.mergeSystem = mergeSystem;
            this.edgeBlockEquipSystem = edgeBlockEquipSystem;
            this.clusterSeparationSystem = clusterSeparationSystem;
            this.substanceDockSystem = substanceDockSystem;
            this.substanceDefinitionRegistry = substanceDefinitionRegistry;
            this.viewRegistry = viewRegistry;
            this.selectionVisualService = selectionVisualService;
        }

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Pointer.current == null)
                return;

            if (Pointer.current.press.wasPressedThisFrame)
                BeginPress();

            if (Pointer.current.press.isPressed)
                UpdatePress();

            if (Pointer.current.press.wasReleasedThisFrame)
                EndPress();
        }

        private void BeginPress()
        {
            if (IsPointerOverUI())
                return;

            pendingView = RaycastSubstanceView();

            if (pendingView == null)
                return;

            pointerDownPosition = GetPointerWorldPosition();
            dragStartPosition = pendingView.transform.position;
            pointerToViewOffset = (Vector2)pendingView.transform.position - pointerDownPosition;
            dragOrigin = default;

            selectionVisualService.SelectSubstance(pendingView);
        }

        public void BeginExternalDrag(SubstanceView view)
        {
            if (view == null)
                return;

            pendingView = null;
            draggingView = view;

            Vector2 pointerPosition = GetPointerWorldPosition();
            pointerDownPosition = pointerPosition;
            dragStartPosition = pointerPosition;
            pointerToViewOffset = Vector2.zero;

            selectionVisualService.SelectSubstance(draggingView);
            dragOrigin = substanceDockSystem.BeginDrag(draggingView.EntityId);
            draggingView.SetPressed(true);
            stackSystem.TryMove(draggingView.EntityId, pointerPosition);
        }

        private void UpdatePress()
        {
            if (pendingView != null && ShouldStartDrag())
                StartDrag();

            if (draggingView == null)
                return;

            Vector2 position = GetPointerWorldPosition() + pointerToViewOffset;
            stackSystem.TryMove(draggingView.EntityId, position);
        }

        private bool ShouldStartDrag()
        {
            return (GetPointerWorldPosition() - pointerDownPosition).sqrMagnitude >=
                dragStartThreshold * dragStartThreshold;
        }

        private void StartDrag()
        {
            draggingView = pendingView;
            pendingView = null;

            dragOrigin = substanceDockSystem.BeginDrag(draggingView.EntityId);
            draggingView.SetPressed(true);
        }

        private void EndPress()
        {
            if (draggingView != null)
            {
                EndDrag();
                return;
            }

            pendingView = null;
            dragOrigin = default;
        }

        private void EndDrag()
        {
            if (draggingView == null)
                return;

            var inputNode = RaycastInputNodeView();

            if (inputNode != null)
            {
                FinishDrop(DropOnInputNode(inputNode));
                return;
            }

            var mergeNode = RaycastMergeNodeView();

            if (mergeNode != null)
            {
                FinishDrop(DropOnMergeNode(mergeNode));
                return;
            }

            var edge = RaycastEdgeView();

            if (edge != null)
            {
                FinishDrop(DropOnEdge(edge));
                return;
            }

            var trashCan = RaycastTrashCanView();

            if (trashCan != null)
            {
                FinishDrop(DropOnTrashCan());
                return;
            }

            if (substanceDockSystem.TryGetDockAt(GetPointerWorldPosition(), out var dockKind) &&
                substanceDockSystem.TryDock(draggingView.EntityId, dockKind))
            {
                FinishDrop(DropResult.Docked);
                return;
            }

            if (RaycastAnyNodeView())
            {
                FinishDrop(DropResult.ReturnToOrigin);
                return;
            }

            FinishDrop(DropResult.Free);
        }

        private void FinishDrop(DropResult result)
        {
            switch (result)
            {
                case DropResult.Docked:
                case DropResult.Accepted:
                    ReleaseDraggingView(false, false);
                    break;
                case DropResult.ReturnToOrigin:
                    RestoreDraggingViewOrigin();
                    break;
                case DropResult.Removed:
                    ClearDragState();
                    break;
                default:
                    ReleaseDraggingView(true, true);
                    break;
            }
        }

        private void ReleaseDraggingView(bool markFree, bool relax)
        {
            if (draggingView != null)
            {
                if (markFree)
                    substanceDockSystem.MarkFree(draggingView.EntityId);

                substanceDockSystem.EndDrag(draggingView.EntityId);
                draggingView.SetPressed(false);

                if (relax)
                    clusterSeparationSystem.RelaxAround(draggingView.EntityId);
            }

            ClearDragState();
        }

        private void RestoreDraggingViewOrigin()
        {
            if (draggingView == null)
                return;

            stackSystem.TryMove(draggingView.EntityId, dragStartPosition);
            substanceDockSystem.RestoreDragOrigin(draggingView.EntityId, dragOrigin);
            substanceDockSystem.EndDrag(draggingView.EntityId);
            draggingView.SetPressed(false);

            if (!dragOrigin.WasDocked)
                clusterSeparationSystem.RelaxAround(draggingView.EntityId);

            ClearDragState();
        }

        private DropResult DropOnInputNode(NodeView inputNode)
        {
            if (!world.SubstanceStacks.TryGetValue(draggingView.EntityId, out var stack))
            {
                Debug.LogWarning($"[SubstanceDrop] Failed stack={draggingView.EntityId} targetNode={inputNode.EntityId} reason=StackNotFound");
                return DropResult.ReturnToOrigin;
            }

            if (!substanceDefinitionRegistry.TryGet(stack.SubstanceId, out var definition))
            {
                Debug.LogWarning($"[SubstanceDrop] Failed stack={draggingView.EntityId} substance={stack.SubstanceId} targetNode={inputNode.EntityId} reason=SubstanceDefinitionNotFound");
                return DropResult.ReturnToOrigin;
            }

            if (!SubstanceFlowSpawnRule.CanSpawnFlow(world, inputNode.EntityId, definition.Kind))
            {
                Debug.LogWarning($"[SubstanceDrop] Failed stack={draggingView.EntityId} substance={stack.SubstanceId} targetNode={inputNode.EntityId} reason=SubstanceCannotSpawnFlow kind={definition.Kind}");
                return DropResult.ReturnToOrigin;
            }

            if (!world.EnqueueFlowSpawn(inputNode.EntityId, stack.SubstanceId))
            {
                Debug.LogWarning($"[SubstanceDrop] Failed stack={draggingView.EntityId} substance={stack.SubstanceId} targetNode={inputNode.EntityId} reason=FlowSpawnRejected");
                return DropResult.ReturnToOrigin;
            }

            if (!stackSystem.TryConsume(draggingView.EntityId, out var removed))
            {
                Debug.LogWarning($"[SubstanceDrop] Failed stack={draggingView.EntityId} substance={stack.SubstanceId} targetNode={inputNode.EntityId} reason=ConsumeFailed");
                return DropResult.ReturnToOrigin;
            }

            //InputNodeMaterialView materialView = inputNode.GetComponent<InputNodeMaterialView>();

            //if (materialView != null)
            //    materialView.SetMaterialName(definition.DisplayName);

            Debug.Log($"[SubstanceDrop] Succeeded stack={draggingView.EntityId} substance={stack.SubstanceId} targetNode={inputNode.EntityId}");

            if (removed || stackSystem.IsEmpty(draggingView.EntityId))
            {
                RemoveDraggedView();
                return DropResult.Removed;
            }

            return DropResult.ReturnToOrigin;
        }

        private DropResult DropOnMergeNode(NodeView mergeNode)
        {
            if (mergeSystem.TryAddStack(mergeNode.EntityId, draggingView.EntityId))
            {
                Debug.Log($"[SubstanceDrop] Succeeded stack={draggingView.EntityId} targetNode={mergeNode.EntityId} target=Merge");
                return DropResult.Accepted;
            }

            Debug.LogWarning($"[SubstanceDrop] Failed stack={draggingView.EntityId} targetNode={mergeNode.EntityId} reason=MergeRejected");
            return DropResult.ReturnToOrigin;
        }

        private DropResult DropOnEdge(EdgeView edge)
        {
            if (edgeBlockEquipSystem.TryEquip(edge.EntityId, draggingView.EntityId))
            {
                Debug.Log($"[SubstanceDrop] Succeeded stack={draggingView.EntityId} targetEdge={edge.EntityId} target=EdgeBlock");

                if (!world.SubstanceStacks.ContainsKey(draggingView.EntityId))
                {
                    RemoveDraggedView();
                    return DropResult.Removed;
                }

                return DropResult.ReturnToOrigin;
            }

            Debug.LogWarning($"[SubstanceDrop] Failed stack={draggingView.EntityId} targetEdge={edge.EntityId} reason=EdgeBlockRejected");
            return DropResult.ReturnToOrigin;
        }

        private SubstanceView RaycastSubstanceView()
        {
            var hits = Physics2D.RaycastAll(
                GetPointerWorldPosition(),
                Vector2.zero,
                Mathf.Infinity,
                substanceLayer);

            SubstanceView frontView = null;
            float frontZ = float.PositiveInfinity;

            foreach (var hit in hits)
            {
                var view = hit.collider.GetComponentInParent<SubstanceView>();

                if (view == null)
                    continue;

                if (view.transform.position.z >= frontZ)
                    continue;

                frontView = view;
                frontZ = view.transform.position.z;
            }

            return frontView;
        }

        private NodeView RaycastInputNodeView()
        {
            var hits = Physics2D.RaycastAll(GetPointerWorldPosition(), Vector2.zero);

            foreach (var hit in hits)
            {
                var nodeView = hit.collider.GetComponentInParent<NodeView>();

                if (nodeView == null)
                    continue;

                if (!world.Nodes.TryGetValue(nodeView.EntityId, out var node))
                    continue;

                if (node.Category == NodeCategory.Input)
                    return nodeView;
            }

            return null;
        }

        private NodeView RaycastMergeNodeView()
        {
            var hits = Physics2D.RaycastAll(GetPointerWorldPosition(), Vector2.zero);

            foreach (var hit in hits)
            {
                var nodeView = hit.collider.GetComponentInParent<NodeView>();

                if (nodeView == null)
                    continue;

                if (!world.Nodes.TryGetValue(nodeView.EntityId, out var node))
                    continue;

                if (node.Category == NodeCategory.Merge)
                    return nodeView;
            }

            return null;
        }

        private EdgeView RaycastEdgeView()
        {
            var hits = Physics2D.RaycastAll(GetPointerWorldPosition(), Vector2.zero);

            foreach (var hit in hits)
            {
                var edgeView = hit.collider.GetComponentInParent<EdgeView>();

                if (edgeView != null)
                    return edgeView;
            }

            return null;
        }

        private DropResult DropOnTrashCan()
        {
            if (!world.SubstanceStacks.TryGetValue(draggingView.EntityId, out var stack))
                return DropResult.ReturnToOrigin;

            if (stack.IsInfinite)
                return DropResult.ReturnToOrigin;

            var stackId = draggingView.EntityId;
            RemoveDraggedView();
            substanceDockSystem.EndDrag(stackId);
            return DropResult.Removed;
        }

        private bool RaycastAnyNodeView()
        {
            var hits = Physics2D.RaycastAll(GetPointerWorldPosition(), Vector2.zero);

            foreach (var hit in hits)
                if (hit.collider.GetComponentInParent<NodeView>() != null)
                    return true;

            return false;
        }

        private TrashCanView RaycastTrashCanView()
        {
            var hits = Physics2D.RaycastAll(GetPointerWorldPosition(), Vector2.zero);

            foreach (var hit in hits)
            {
                var trashCan = hit.collider.GetComponentInParent<TrashCanView>();
                if (trashCan != null)
                    return trashCan;
            }

            return null;
        }

        private void RemoveDraggedView()
        {
            if (draggingView == null)
                return;

            var stackId = draggingView.EntityId;
            stackSystem.Remove(stackId);

            if (viewRegistry.TryGetView(stackId, out var view))
            {
                viewRegistry.Unregister(stackId);
                Object.Destroy(view.gameObject);
            }

            draggingView = null;
        }

        private void ClearDragState()
        {
            pendingView = null;
            draggingView = null;
            dragOrigin = default;
        }

        private Vector2 GetPointerWorldPosition()
        {
            EnsureCamera();
            Vector2 screenPosition = Pointer.current.position.ReadValue();
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
            return new Vector2(worldPosition.x, worldPosition.y);
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
    }
}
