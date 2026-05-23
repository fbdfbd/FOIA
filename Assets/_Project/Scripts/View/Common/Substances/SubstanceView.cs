using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Definitions;
using OneMoreSpoon.View.Common;
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
        [SerializeField] private float normalZ = 0f;
        [SerializeField] private float pressedZOffset = -0.5f;

        private SubstanceDefinitionRegistry definitionRegistry;
        private bool isPressed;

        public void Initialize(SubstanceDefinitionRegistry definitionRegistry)
        {
            this.definitionRegistry = definitionRegistry;
        }

        public void SetVisuals(
            SpriteRenderer backgroundRenderer,
            TMP_Text nameText,
            TMP_Text amountText)
        {
            this.backgroundRenderer = backgroundRenderer;
            this.nameText = nameText;
            this.amountText = amountText;
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
                float z = isPressed ? normalZ + pressedZOffset : normalZ;
                transform.position = new Vector3(position.Value.x, position.Value.y, z);
            }

            UpdateText();
        }

        public void SetPressed(bool pressed)
        {
            isPressed = pressed;
        }

        private void UpdateText()
        {
            if (!World.SubstanceStacks.TryGetValue(EntityId, out var stack))
                return;

            if (nameText != null &&
                definitionRegistry != null &&
                definitionRegistry.TryGet(stack.SubstanceId, out var definition))
            {
                nameText.text = definition.DisplayName;
            }

            if (amountText != null)
                amountText.text = stack.IsInfinite ? "INF" : $"x{stack.Amount}";
        }
    }
}
