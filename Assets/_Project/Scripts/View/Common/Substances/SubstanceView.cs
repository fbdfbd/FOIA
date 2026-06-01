using DG.Tweening;
using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Definitions;
using OneMoreSpoon.Game.Systems;
using OneMoreSpoon.Presenter;
using OneMoreSpoon.View.Common;
using TMPro;
using UnityEngine;

namespace OneMoreSpoon.View.Substances
{
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class SubstanceView : EntityView, ISelectableView
    {
        [SerializeField] private SpriteRenderer backgroundRenderer;
        [SerializeField] private SpriteRenderer imageRenderer;
        [SerializeField] private SelectableOutlineVisual selectionVisual;
        [SerializeField] private SortingLayerStateVisual sortingLayerVisual;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text amountText;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private float normalZ = 0f;
        [SerializeField] private float pressedZOffset = -0.5f;
        [SerializeField] private float moveTweenDuration = 0.14f;

        private SubstanceDefinitionRegistry definitionRegistry;
        private SubstanceDockDepthState dockDepthState;
        private SubstanceTitleProvider titleProvider;
        private bool isPressed;
        private bool hasTargetPosition;
        private Vector3 lastTargetPosition;
        private Tween moveTween;

        public void Initialize(
            SubstanceDefinitionRegistry definitionRegistry,
            SubstanceDockDepthState dockDepthState,
            SubstanceTitleProvider titleProvider)
        {
            this.definitionRegistry = definitionRegistry;
            this.dockDepthState = dockDepthState;
            this.titleProvider = titleProvider;
        }

        public void SetVisuals(
            SpriteRenderer backgroundRenderer,
            TMP_Text nameText,
            TMP_Text amountText,
            TMP_Text titleText)
        {
            this.backgroundRenderer = backgroundRenderer;
            this.nameText = nameText;
            this.amountText = amountText;
            this.titleText = titleText;
        }

        private void Awake()
        {
            if (backgroundRenderer == null)
                backgroundRenderer = GetComponent<SpriteRenderer>();

            if (selectionVisual == null)
                selectionVisual = GetComponent<SelectableOutlineVisual>();

            if (sortingLayerVisual == null)
                sortingLayerVisual = GetComponent<SortingLayerStateVisual>()
                    ?? gameObject.AddComponent<SortingLayerStateVisual>();
        }

        private void LateUpdate()
        {
            if (World == null)
                return;

            if (World.Positions.TryGetValue(EntityId, out var position))
            {
                float z = normalZ + (dockDepthState?.GetDepth(EntityId) ?? 0f);

                if (isPressed)
                    z += pressedZOffset;

                Vector3 targetPosition = new(position.Value.x, position.Value.y, z);
                SyncPosition(targetPosition);
            }

            UpdateText();
        }

        private void SyncPosition(Vector3 targetPosition)
        {
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

        public void SetPressed(bool pressed)
        {
            isPressed = pressed;
            sortingLayerVisual?.SetActiveLayer(pressed);
        }

        public void SetSelected(bool selected)
        {
            selectionVisual?.SetSelected(selected);
        }

        public void SetOutlineColors(SubstanceOutlineColors colors)
        {
            selectionVisual?.SetColors(colors.Normal, colors.Selected);
        }

        public void SetImage(Sprite image)
        {
            if (imageRenderer == null)
                return;

            imageRenderer.sprite = image;
            imageRenderer.gameObject.SetActive(image != null);
        }

        private void UpdateText()
        {
            if (!World.SubstanceStacks.TryGetValue(EntityId, out var stack))
                return;

            if (definitionRegistry == null || !definitionRegistry.TryGet(stack.SubstanceId, out SO_SubstanceDefinition definition))
                return;

            if (nameText != null)
                nameText.text = definition.DisplayName;

            if (amountText != null)
                amountText.text = stack.IsInfinite ? "INF" : $"x{stack.Amount}";

            if (titleText != null)
                titleText.text = titleProvider?.GetTitle(definition.Kind) ?? string.Empty;
        }
    }
}
