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

        private SubstanceDefinitionRegistry definitionRegistry;

        public void Initialize(SubstanceDefinitionRegistry definitionRegistry)
        {
            this.definitionRegistry = definitionRegistry;
        }

        private void LateUpdate()
        {
            if (World == null)
                return;

            if (!World.Flows.TryGetValue(EntityId, out var flow))
                return;

            if (TryGetPosition(flow, out var position))
                transform.position = position;

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

            if (definitionRegistry != null &&
                definitionRegistry.TryGet(substance.SubstanceId, out var definition))
            {
                labelText.text = definition.DisplayName;
                return;
            }

            labelText.text = substance.SubstanceId;
        }
    }
}
