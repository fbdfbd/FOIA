using System;
using FOIA.Input;
using FOIA.Runtime;
using R3;
using UnityEngine;
using VContainer;

namespace FOIA.Presentation
{
    public sealed class EdgeBlockDragController : MonoBehaviour
    {
        [SerializeField] private BoardPointerInput pointerInput;
        [SerializeField] private float dragZ = 0f;

        private ProcessBoardState boardState;
        private IDisposable downSubscription;
        private IDisposable moveSubscription;
        private IDisposable upSubscription;
        private EdgeBlockView draggingBlock;
        private Vector3 dragOffset;
        private Vector3 dragStartPosition;
        private Transform dragStartParent;

        [Inject]
        public void Construct(ProcessBoardState boardState)
        {
            this.boardState = boardState;
        }

        public void ConfigureRuntime(ProcessBoardState boardState)
        {
            this.boardState = boardState;
        }

        private void Awake()
        {
            if (pointerInput == null)
                pointerInput = FindFirstObjectByType<BoardPointerInput>();
        }

        private void OnEnable()
        {
            if (pointerInput == null)
                return;

            downSubscription = pointerInput.PointerDown.Subscribe(OnPointerDown);
            moveSubscription = pointerInput.PointerMove.Subscribe(OnPointerMove);
            upSubscription = pointerInput.PointerUp.Subscribe(OnPointerUp);
        }

        private void OnDisable()
        {
            downSubscription?.Dispose();
            moveSubscription?.Dispose();
            upSubscription?.Dispose();
        }

        private void OnPointerDown(BoardPointerEvent pointerEvent)
        {
            if (pointerEvent.Target is not EdgeBlockView block || block.Definition == null)
                return;

            draggingBlock = block;
            dragStartPosition = block.transform.position;
            dragStartParent = block.transform.parent;
            dragOffset = block.transform.position - WithDragZ(pointerEvent.WorldPosition);
        }

        private void OnPointerMove(BoardPointerEvent pointerEvent)
        {
            if (draggingBlock == null)
                return;

            draggingBlock.transform.position = WithDragZ(pointerEvent.WorldPosition) + dragOffset;
        }

        private void OnPointerUp(BoardPointerEvent pointerEvent)
        {
            if (draggingBlock == null)
                return;

            var block = draggingBlock;
            draggingBlock = null;

            if (pointerEvent.Target is EdgeSlotView slot
                && slot.Definition != null
                && boardState != null
                && boardState.EquipBlock(slot.EdgeId, block.BlockId))
            {
                block.transform.SetParent(slot.AttachPoint, true);
                block.transform.position = slot.AttachPoint.position;
                return;
            }

            block.transform.SetParent(dragStartParent, true);
            block.transform.position = dragStartPosition;
        }

        private Vector3 WithDragZ(Vector3 position)
        {
            position.z = dragZ;
            return position;
        }
    }
}
