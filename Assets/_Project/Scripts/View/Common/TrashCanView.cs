using UnityEngine;
using UnityEngine.InputSystem;

namespace OneMoreSpoon.View.Common
{
    public sealed class TrashCanView : MonoBehaviour
    {
        private Camera mainCamera;
        private Collider2D col;
        private bool isDragging;
        private Vector2 dragOffset;

        private void Awake()
        {
            mainCamera = Camera.main;
            col = GetComponent<Collider2D>();
        }

        private void Update()
        {
            if (Pointer.current == null)
                return;

            if (Pointer.current.press.wasPressedThisFrame)
                TryBeginDrag();

            if (isDragging && Pointer.current.press.isPressed)
                Drag();

            if (Pointer.current.press.wasReleasedThisFrame)
                isDragging = false;
        }

        private void TryBeginDrag()
        {
            Vector2 worldPos = GetPointerWorldPosition();

            if (col == null || !col.OverlapPoint(worldPos))
                return;

            isDragging = true;
            dragOffset = (Vector2)transform.position - worldPos;
        }

        private void Drag()
        {
            Vector2 worldPos = GetPointerWorldPosition();
            transform.position = new Vector3(worldPos.x + dragOffset.x, worldPos.y + dragOffset.y, transform.position.z);
        }

        private Vector2 GetPointerWorldPosition()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            Vector2 screenPos = Pointer.current.position.ReadValue();
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
            return new Vector2(worldPos.x, worldPos.y);
        }
    }
}
