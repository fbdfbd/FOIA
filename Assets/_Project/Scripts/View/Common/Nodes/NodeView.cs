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
        [SerializeField] private SelectableOutlineVisual selectionVisual;
        [SerializeField] private SortingLayerStateVisual sortingLayerVisual;
        [SerializeField] private float normalZ = 0f;
        [SerializeField] private float pressedZOffset = -0.5f;
        [SerializeField] private float moveTweenDuration = 0.14f;

        private bool isPressed;
        private bool hasTargetPosition;
        private Vector3 lastTargetPosition;
        private Tween moveTween;
        private string currentLabelText;
        private Sprite currentImage;
        private bool hasImageState;

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            if (selectionVisual == null)
                selectionVisual = GetComponent<SelectableOutlineVisual>();

            if (sortingLayerVisual == null)
                sortingLayerVisual = GetComponent<SortingLayerStateVisual>()
                    ?? gameObject.AddComponent<SortingLayerStateVisual>();

            if (label == null)
                label = GetComponentInChildren<TMP_Text>();
        }

        public void RenderPosition(Vector3 targetPosition, bool animate)
        {
            if (!animate || isPressed)
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

        public Vector3 GetRenderPosition(Vector2 worldPosition)
        {
            float z = isPressed ? normalZ + pressedZOffset : normalZ;
            return new Vector3(worldPosition.x, worldPosition.y, z);
        }

        private void OnDestroy()
        {
            moveTween?.Kill();
        }

        public void SetSelected(bool selected)
        {
            selectionVisual?.SetSelected(selected);
        }

        public void SetPressed(bool pressed)
        {
            isPressed = pressed;
            sortingLayerVisual?.SetActiveLayer(pressed);

            if (World != null && World.Positions.TryGetValue(EntityId, out var position))
                RenderPosition(GetRenderPosition(position.Value), false);
        }

        public void SetLabel(string text)
        {
            if (label == null || currentLabelText == text)
                return;

            currentLabelText = text;
            label.text = text;
        }

        public void SetImage(Sprite image)
        {
            if (imageRenderer == null)
                return;

            if (hasImageState && currentImage == image)
                return;

            currentImage = image;
            hasImageState = true;
            imageRenderer.sprite = image;
            imageRenderer.gameObject.SetActive(image != null);
        }
    }
}
