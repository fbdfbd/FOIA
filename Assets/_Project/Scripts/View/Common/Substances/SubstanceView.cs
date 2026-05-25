using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Definitions;
using OneMoreSpoon.View.Common;
using OneMoreSpoon.Game.Components;
using OneMoreSpoon.Game.Systems;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace OneMoreSpoon.View.Substances
{
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class SubstanceView : EntityView
    {
        [SerializeField] private SpriteRenderer backgroundRenderer;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text amountText;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private float normalZ = 0f;
        [SerializeField] private float pressedZOffset = -0.5f;
        [SerializeField] private float moveTweenDuration = 0.14f;

        private SubstanceDefinitionRegistry definitionRegistry;
        private SubstanceDockDepthState dockDepthState;
        private bool isPressed;
        private bool hasTargetPosition;
        private Vector3 lastTargetPosition;
        private Tween moveTween;

        public void Initialize(
            SubstanceDefinitionRegistry definitionRegistry,
            SubstanceDockDepthState dockDepthState)
        {
            this.definitionRegistry = definitionRegistry;
            this.dockDepthState = dockDepthState;
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
        }

        private void UpdateText()
        {
            if (!World.SubstanceStacks.TryGetValue(EntityId, out var stack))
                return;

            if (definitionRegistry == null || !definitionRegistry.TryGet(stack.SubstanceId, out SO_SubstanceDefinition definition))
            {
                return;
            }

            if (nameText != null)
                nameText.text = definition.DisplayName;

            if (amountText != null)
                amountText.text = stack.IsInfinite ? "INF" : $"x{stack.Amount}";

            if (titleText != null)
                titleText.text = GetTitle(definition.Kind);
        }

        private string GetTitle(SubstanceKind kind)
        {
            switch (kind)
            {
                case SubstanceKind.EdgeBlock:
                    return "엣지블럭";

                case SubstanceKind.TraitShard:
                    return "부산물";

                case SubstanceKind.SourceMaterial:
                    return "원재료";

                case SubstanceKind.Dish:
                    return "요리";

                case SubstanceKind.FinalDish:
                    return "최종요리";

                case SubstanceKind.Material:
                    return "재료";

                default:
                    return string.Empty;
            }
        }
    }
}
