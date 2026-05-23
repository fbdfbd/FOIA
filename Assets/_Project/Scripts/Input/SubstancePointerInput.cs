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

        private GameWorld world;
        private SubstanceStackSystem stackSystem;
        private MergeSystem mergeSystem;
        private EdgeBlockEquipSystem edgeBlockEquipSystem;
        private SubstanceDefinitionRegistry substanceDefinitionRegistry;
        private ViewRegistry viewRegistry;
        private SelectionVisualService selectionVisualService;

        private Camera mainCamera;
        private SubstanceView draggingView;
        private Vector2 dragStartPosition;
        private Vector2 pointerToViewOffset;

        [Inject]
        public void Construct(
            GameWorld world,
            SubstanceStackSystem stackSystem,
            MergeSystem mergeSystem,
            EdgeBlockEquipSystem edgeBlockEquipSystem,
            SubstanceDefinitionRegistry substanceDefinitionRegistry,
            ViewRegistry viewRegistry,
            SelectionVisualService selectionVisualService)
        {
            this.world = world;
            this.stackSystem = stackSystem;
            this.mergeSystem = mergeSystem;
            this.edgeBlockEquipSystem = edgeBlockEquipSystem;
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
                BeginDrag();

            if (Pointer.current.press.isPressed)
                Drag();

            if (Pointer.current.press.wasReleasedThisFrame)
                EndDrag();
        }

        private void BeginDrag()
        {
            if (IsPointerOverUI())
                return;

            draggingView = RaycastSubstanceView();

            if (draggingView == null)
                return;

            selectionVisualService.SelectSubstance(draggingView);
            dragStartPosition = draggingView.transform.position;
            pointerToViewOffset = (Vector2)draggingView.transform.position - GetPointerWorldPosition();
        }

        private void Drag()
        {
            if (draggingView == null)
                return;

            Vector2 position = GetPointerWorldPosition() + pointerToViewOffset;
            stackSystem.TryMove(draggingView.EntityId, position);
        }

        private void EndDrag()
        {
            if (draggingView == null)
                return;

            var inputNode = RaycastInputNodeView();

            if (inputNode != null)
            {
                DropOnInputNode(inputNode);
                draggingView = null;
                return;
            }

            var mergeNode = RaycastMergeNodeView();

            if (mergeNode != null)
            {
                DropOnMergeNode(mergeNode);
                draggingView = null;
                return;
            }

            var edge = RaycastEdgeView();

            if (edge != null)
            {
                DropOnEdge(edge);
                draggingView = null;
                return;
            }

            if (RaycastBlockingObject())
                stackSystem.TryMove(draggingView.EntityId, dragStartPosition);

            draggingView = null;
        }

        private void DropOnInputNode(NodeView inputNode)
        {
            if (!world.SubstanceStacks.TryGetValue(draggingView.EntityId, out var stack))
            {
                Debug.LogWarning($"[SubstanceDrop] Failed stack={draggingView.EntityId} targetNode={inputNode.EntityId} reason=StackNotFound");
                stackSystem.TryMove(draggingView.EntityId, dragStartPosition);
                return;
            }

            if (!substanceDefinitionRegistry.TryGet(stack.SubstanceId, out var definition))
            {
                Debug.LogWarning($"[SubstanceDrop] Failed stack={draggingView.EntityId} substance={stack.SubstanceId} targetNode={inputNode.EntityId} reason=SubstanceDefinitionNotFound");
                stackSystem.TryMove(draggingView.EntityId, dragStartPosition);
                return;
            }

            if (!SubstanceFlowSpawnRule.CanSpawnFlow(definition.Kind))
            {
                Debug.LogWarning($"[SubstanceDrop] Failed stack={draggingView.EntityId} substance={stack.SubstanceId} targetNode={inputNode.EntityId} reason=SubstanceCannotSpawnFlow kind={definition.Kind}");
                stackSystem.TryMove(draggingView.EntityId, dragStartPosition);
                return;
            }

            if (!world.EnqueueFlowSpawn(inputNode.EntityId, stack.SubstanceId))
            {
                Debug.LogWarning($"[SubstanceDrop] Failed stack={draggingView.EntityId} substance={stack.SubstanceId} targetNode={inputNode.EntityId} reason=FlowSpawnRejected");
                stackSystem.TryMove(draggingView.EntityId, dragStartPosition);
                return;
            }

            if (!stackSystem.TryConsume(draggingView.EntityId))
            {
                Debug.LogWarning($"[SubstanceDrop] Failed stack={draggingView.EntityId} substance={stack.SubstanceId} targetNode={inputNode.EntityId} reason=ConsumeFailed");
                stackSystem.TryMove(draggingView.EntityId, dragStartPosition);
                return;
            }

            Debug.Log($"[SubstanceDrop] Succeeded stack={draggingView.EntityId} substance={stack.SubstanceId} targetNode={inputNode.EntityId}");

            if (stackSystem.IsEmpty(draggingView.EntityId))
            {
                RemoveDraggedView();
                return;
            }

            stackSystem.TryMove(draggingView.EntityId, dragStartPosition);
        }

        private void DropOnMergeNode(NodeView mergeNode)
        {
            if (mergeSystem.TryAddStack(mergeNode.EntityId, draggingView.EntityId))
            {
                Debug.Log($"[SubstanceDrop] Succeeded stack={draggingView.EntityId} targetNode={mergeNode.EntityId} target=Merge");
                return;
            }

            Debug.LogWarning($"[SubstanceDrop] Failed stack={draggingView.EntityId} targetNode={mergeNode.EntityId} reason=MergeRejected");
            stackSystem.TryMove(draggingView.EntityId, dragStartPosition);
        }

        private void DropOnEdge(EdgeView edge)
        {
            if (edgeBlockEquipSystem.TryEquip(edge.EntityId, draggingView.EntityId))
            {
                Debug.Log($"[SubstanceDrop] Succeeded stack={draggingView.EntityId} targetEdge={edge.EntityId} target=EdgeBlock");

                if (!world.SubstanceStacks.ContainsKey(draggingView.EntityId))
                    RemoveDraggedView();
                else
                    stackSystem.TryMove(draggingView.EntityId, dragStartPosition);

                return;
            }

            Debug.LogWarning($"[SubstanceDrop] Failed stack={draggingView.EntityId} targetEdge={edge.EntityId} reason=EdgeBlockRejected");
            stackSystem.TryMove(draggingView.EntityId, dragStartPosition);
        }

        private SubstanceView RaycastSubstanceView()
        {
            var hit = Physics2D.Raycast(
                GetPointerWorldPosition(),
                Vector2.zero,
                Mathf.Infinity,
                substanceLayer);

            if (hit.collider == null)
                return null;

            return hit.collider.GetComponentInParent<SubstanceView>();
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

        private bool RaycastBlockingObject()
        {
            var hits = Physics2D.RaycastAll(GetPointerWorldPosition(), Vector2.zero);

            foreach (var hit in hits)
            {
                var view = hit.collider.GetComponentInParent<EntityView>();

                if (view == null)
                    continue;

                if (draggingView != null && view.EntityId == draggingView.EntityId)
                    continue;

                return true;
            }

            return false;
        }

        private void RemoveDraggedView()
        {
            var stackId = draggingView.EntityId;
            stackSystem.Remove(stackId);

            if (viewRegistry.TryGetView(stackId, out var view))
            {
                viewRegistry.Unregister(stackId);
                Object.Destroy(view.gameObject);
            }
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
