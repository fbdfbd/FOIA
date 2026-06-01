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
        private string lastSubstanceId;
        private int lastAmount;
        private bool lastIsInfinite;
        private string lastNameText;
        private string lastAmountText;
        private string lastTitleText;
        private bool hasTextState;
        private Sprite currentImage;
        private bool hasImageState;

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
            float z = normalZ + (dockDepthState?.GetDepth(EntityId) ?? 0f);

            if (isPressed)
                z += pressedZOffset;

            return new Vector3(worldPosition.x, worldPosition.y, z);
        }

        private void OnDestroy()
        {
            moveTween?.Kill();
        }

        public void SetPressed(bool pressed)
        {
            isPressed = pressed;
            sortingLayerVisual?.SetActiveLayer(pressed);

            if (World != null && World.Positions.TryGetValue(EntityId, out var position))
                RenderPosition(GetRenderPosition(position.Value), false);
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

            if (hasImageState && currentImage == image)
                return;

            currentImage = image;
            hasImageState = true;
            imageRenderer.sprite = image;
            imageRenderer.gameObject.SetActive(image != null);
        }

        public void RefreshTextFromWorld()
        {
            if (!World.SubstanceStacks.TryGetValue(EntityId, out var stack))
                return;

            if (definitionRegistry == null || !definitionRegistry.TryGet(stack.SubstanceId, out SO_SubstanceDefinition definition))
                return;

            bool stackTextChanged =
                !hasTextState ||
                lastSubstanceId != stack.SubstanceId ||
                lastAmount != stack.Amount ||
                lastIsInfinite != stack.IsInfinite;

            if (!stackTextChanged)
                return;

            string nextNameText = definition.DisplayName;
            string nextAmountText = stack.IsInfinite ? "INF" : $"x{stack.Amount}";
            string nextTitleText = titleProvider?.GetTitle(definition.Kind) ?? string.Empty;

            SetTextIfChanged(nameText, ref lastNameText, nextNameText);
            SetTextIfChanged(amountText, ref lastAmountText, nextAmountText);
            SetTextIfChanged(titleText, ref lastTitleText, nextTitleText);

            lastSubstanceId = stack.SubstanceId;
            lastAmount = stack.Amount;
            lastIsInfinite = stack.IsInfinite;
            hasTextState = true;
        }

        private static void SetTextIfChanged(TMP_Text target, ref string currentText, string nextText)
        {
            if (target == null || currentText == nextText)
                return;

            currentText = nextText;
            target.text = nextText;
        }
    }
}
