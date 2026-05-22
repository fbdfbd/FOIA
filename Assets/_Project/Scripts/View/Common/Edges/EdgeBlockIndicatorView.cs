using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Definitions;
using OneMoreSpoon.View.Common;
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

        private void LateUpdate()
        {
            if (World == null)
                return;

            if (!World.Edges.TryGetValue(EntityId, out var edge))
                return;

            if (!World.Positions.TryGetValue(edge.FromNodeId, out var fromPosition))
                return;

            if (!World.Positions.TryGetValue(edge.ToNodeId, out var toPosition))
                return;

            UpdateTransform(fromPosition.Value, toPosition.Value);
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
                labelText.text = string.Empty;
                return;
            }

            if (definitionRegistry != null &&
                definitionRegistry.TryGet(slot.EquippedSubstanceId, out var definition))
            {
                labelText.text = definition.DisplayName;
                return;
            }

            labelText.text = slot.EquippedSubstanceId;
        }
    }
}
