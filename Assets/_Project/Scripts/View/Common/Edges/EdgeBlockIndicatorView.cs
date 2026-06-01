using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Components;
using OneMoreSpoon.Game.Definitions;
using OneMoreSpoon.View.Common;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace OneMoreSpoon.View.Edges
{
    public sealed class EdgeBlockIndicatorView : EntityView
    {
        [SerializeField] private TMP_Text labelText;
        [SerializeField] private float edgeT = 0.5f;
        [SerializeField] private float perpendicularOffset = 0.28f;
        [SerializeField] private bool rotateWithEdge;

        private SubstanceDefinitionRegistry definitionRegistry;
        private readonly List<string> lastSubstanceIds = new();
        private readonly StringBuilder labelBuilder = new();
        private string lastLabelText = string.Empty;

        public void Initialize(SubstanceDefinitionRegistry definitionRegistry)
        {
            this.definitionRegistry = definitionRegistry;
        }

        public void SetVisuals(TMP_Text labelText)
        {
            this.labelText = labelText;
        }

        private void Awake()
        {
            if (labelText == null)
                labelText = GetComponentInChildren<TMP_Text>();
        }

        public void RenderIndicator(EdgeComponent edge, Vector2 fromPosition, Vector2 toPosition)
        {
            UpdateTransform(fromPosition, toPosition);
            UpdateLabel();
        }

        private void UpdateTransform(Vector2 from, Vector2 to)
        {
            Vector2 direction = to - from;
            Vector2 position = Vector2.Lerp(from, to, Mathf.Clamp01(edgeT));

            if (direction.sqrMagnitude > 0.001f)
            {
                Vector2 normal = new(-direction.y, direction.x);
                normal.Normalize();
                position += normal * perpendicularOffset;

                if (rotateWithEdge)
                {
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0f, 0f, angle);
                }
                else
                {
                    transform.rotation = Quaternion.identity;
                }
            }

            transform.position = position;
        }

        private void UpdateLabel()
        {
            if (labelText == null)
                return;

            if (!World.EdgeBlockSlots.TryGetValue(EntityId, out var slot) || !slot.HasBlock)
            {
                SetLabelIfChanged(string.Empty);
                lastSubstanceIds.Clear();
                return;
            }

            if (!HasSlotChanged(slot.EquippedSubstanceIds))
                return;

            labelBuilder.Clear();
            foreach (var substanceId in slot.EquippedSubstanceIds)
            {
                if (labelBuilder.Length > 0)
                    labelBuilder.Append('\n');

                if (definitionRegistry != null &&
                    definitionRegistry.TryGet(substanceId, out var definition))
                {
                    labelBuilder.Append(definition.DisplayName);
                    continue;
                }

                labelBuilder.Append(substanceId);
            }

            RememberSlot(slot.EquippedSubstanceIds);
            SetLabelIfChanged(labelBuilder.ToString());
        }

        private bool HasSlotChanged(IReadOnlyList<string> substanceIds)
        {
            if (lastSubstanceIds.Count != substanceIds.Count)
                return true;

            for (int i = 0; i < substanceIds.Count; i++)
                if (lastSubstanceIds[i] != substanceIds[i])
                    return true;

            return false;
        }

        private void RememberSlot(IReadOnlyList<string> substanceIds)
        {
            lastSubstanceIds.Clear();

            for (int i = 0; i < substanceIds.Count; i++)
                lastSubstanceIds.Add(substanceIds[i]);
        }

        private void SetLabelIfChanged(string text)
        {
            if (lastLabelText == text)
                return;

            lastLabelText = text;
            labelText.text = text;
        }
    }
}
