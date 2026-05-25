using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Systems;
using OneMoreSpoon.View.Common;
using OneMoreSpoon.View.Nodes;
using OneMoreSpoon.View.Substances;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VContainer;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Input
{
    public sealed class MergeSlotDragOutInput : MonoBehaviour
    {
        [SerializeField] private LayerMask mergeSlotLayer;

        private GameWorld world;
        private SubstanceStackSystem stackSystem;
        private Camera mainCamera;

        private ViewRegistry viewRegistry;
        private SubstancePointerInput substancePointerInput;

        [Inject]
        public void Construct(GameWorld world, SubstanceStackSystem stackSystem, ViewRegistry viewRegistry, SubstancePointerInput substancePointerInput)
        {
            this.world = world;
            this.stackSystem = stackSystem;
            this.viewRegistry = viewRegistry;
            this.substancePointerInput = substancePointerInput;
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
                BeginDragOut();
        }

        private void BeginDragOut()
        {
            if (IsPointerOverUI()) return;

            MergeSlotHandle handle = RaycastHandle();

            if (handle == null || handle.SlotIndex != 0) return;

            NodeView mergeNode = handle.GetComponentInParent<NodeView>();

            if (mergeNode == null) return;

            if (!world.MergeSlots.TryGetValue(mergeNode.EntityId, out var slot)) return;

            if (slot.StackIds.Count != 1) return;

            GameEntityId stackId = slot.StackIds[0];
            slot.StackIds.RemoveAt(0);
            slot.MarkResolved();

            stackSystem.TryMove(stackId, GetPointerWorldPosition());

            if (!viewRegistry.TryGetView(stackId, out EntityView entityView)) return;

            SetVisible(entityView.gameObject, true);
            SubstanceView substanceView = entityView.GetComponent<SubstanceView>();

            if (substanceView == null) return;

            substancePointerInput.BeginExternalDrag(substanceView);
        }

        private MergeSlotHandle RaycastHandle()
        {
            Vector2 worldPosition = GetPointerWorldPosition();

            RaycastHit2D hit = Physics2D.Raycast( worldPosition, Vector2.zero, Mathf.Infinity, mergeSlotLayer);

            if (hit.collider == null)
                return null;

            return hit.collider.GetComponentInParent<MergeSlotHandle>();
        }

        private Vector2 GetPointerWorldPosition()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            Vector2 screenPosition = Pointer.current.position.ReadValue();
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);

            return new Vector2(worldPosition.x, worldPosition.y);
        }

        private bool IsPointerOverUI()
        {
            return EventSystem.current != null
                && EventSystem.current.IsPointerOverGameObject();
        }

        private void SetVisible(GameObject target, bool visible)
        {
            foreach (Renderer renderer in target.GetComponentsInChildren<Renderer>())
                renderer.enabled = visible;

            foreach (Canvas canvas in target.GetComponentsInChildren<Canvas>())
                canvas.enabled = visible;

            foreach (Graphic graphic in target.GetComponentsInChildren<Graphic>())
                graphic.enabled = visible;

            foreach (Collider2D collider in target.GetComponentsInChildren<Collider2D>())
                collider.enabled = visible;
        }
    }
}
