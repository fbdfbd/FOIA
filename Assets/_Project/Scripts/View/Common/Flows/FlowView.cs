using OneMoreSpoon.Game.Components;
using OneMoreSpoon.Game.Definitions;
using OneMoreSpoon.View.Common;
using TMPro;
using UnityEngine;

namespace OneMoreSpoon.View.Flows
{
    public sealed class FlowView : EntityView
    {
        [SerializeField] private TMP_Text labelText;
        [SerializeField] private float flowZ = -0.25f;
        [SerializeField] private Vector2 positionOffset = new Vector2(0, -0.15f);

        private SubstanceDefinitionRegistry definitionRegistry;
        private string lastSubstanceId;
        private string lastLabelText;
        private bool hasLabelState;

        public void Initialize(SubstanceDefinitionRegistry definitionRegistry)
        {
            this.definitionRegistry = definitionRegistry;
        }

        public void RenderFlow(FlowComponent flow)
        {
            if (World == null)
                return;

            if (TryGetPosition(flow, out var position))
            {
                Vector2 displayPosition = position + positionOffset;
                transform.position = new Vector3(displayPosition.x, displayPosition.y, flowZ);
            }

            UpdateLabel();
        }

        private bool TryGetPosition(FlowComponent flow, out Vector2 position)
        {
            if (flow.State == FlowState.MovingOnEdge)
                return TryGetEdgePosition(flow, out position);

            return TryGetNodePosition(flow.CurrentNodeId, out position);
        }

        private bool TryGetEdgePosition(FlowComponent flow, out Vector2 position)
        {
            position = default;

            if (!World.Edges.TryGetValue(flow.CurrentEdgeId, out var edge))
                return false;

            if (!TryGetNodePosition(edge.FromNodeId, out var from))
                return false;

            if (!TryGetNodePosition(edge.ToNodeId, out var to))
                return false;

            position = Vector2.Lerp(from, to, Mathf.Clamp01(flow.Progress));
            return true;
        }

        private bool TryGetNodePosition(OneMoreSpoon.Game.Core.EntityId nodeId, out Vector2 position)
        {
            if (!World.Positions.TryGetValue(nodeId, out var nodePosition))
            {
                position = default;
                return false;
            }

            position = nodePosition.Value;
            return true;
        }

        private void UpdateLabel()
        {
            if (labelText == null)
                return;

            if (!World.Substances.TryGetValue(EntityId, out var substance))
                return;

            if (hasLabelState && lastSubstanceId == substance.SubstanceId)
                return;

            lastSubstanceId = substance.SubstanceId;
            hasLabelState = true;

            if (definitionRegistry != null &&
                definitionRegistry.TryGet(substance.SubstanceId, out var definition))
            {
                SetLabelIfChanged(definition.DisplayName);
                return;
            }

            SetLabelIfChanged(substance.SubstanceId);
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
