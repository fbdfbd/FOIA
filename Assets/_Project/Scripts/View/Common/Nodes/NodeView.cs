using OneMoreSpoon.View.Common;
using DG.Tweening;
using UnityEngine;
using TMPro;

namespace OneMoreSpoon.View.Nodes
{
    public sealed class NodeView : EntityView, ISelectableView
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer imageRenderer;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color selectedColor = Color.yellow;
        [SerializeField] private float normalZ = 0f;
        [SerializeField] private float pressedZOffset = -0.5f;
        [SerializeField] private float moveTweenDuration = 0.14f;

        private bool isPressed;
        private bool hasTargetPosition;
        private Vector3 lastTargetPosition;
        private Tween moveTween;

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            if (label == null)
                label = GetComponentInChildren<TMP_Text>();
        }

        private void LateUpdate()
        {
            if (World == null)
                return;

            if (!World.Positions.TryGetValue(EntityId, out var position))
                return;

            float z = isPressed ? normalZ + pressedZOffset : normalZ;
            Vector3 targetPosition = new(position.Value.x, position.Value.y, z);

            if (isPressed)
            {
                moveTween?.Kill();
                moveTween = null;
                transform.position = targetPosition;
                lastTargetPosition = targetPosition;
                hasTargetPosition = true;
                return;
            }

            if (!hasTargetPosition)
            {
                transform.position = targetPosition;
                lastTargetPosition = targetPosition;
                hasTargetPosition = true;
                return;
            }

            if ((lastTargetPosition - targetPosition).sqrMagnitude <= 0.0001f)
                return;

            lastTargetPosition = targetPosition;
            moveTween?.Kill();
            moveTween = transform
                .DOMove(targetPosition, moveTweenDuration)
                .SetEase(Ease.OutQuad);
        }

        private void OnDestroy()
        {
            moveTween?.Kill();
        }

        public void SetSelected(bool selected)
        {
            if (spriteRenderer == null)
                return;

            spriteRenderer.color = selected ? selectedColor : normalColor;
        }

        public void SetPressed(bool pressed)
        {
            isPressed = pressed;
        }

        public void SetLabel(string text)
        {
            if (label == null)
                return;
            label.text = text;
        }

        public void SetImage(Sprite image)
        {
            if (imageRenderer == null)
                return;

            imageRenderer.sprite = image;
            imageRenderer.gameObject.SetActive(image != null);
        }
    }
}
