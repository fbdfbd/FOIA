using System.Collections.Generic;
using FOIA.Flow.Definitions;
using UnityEngine;

namespace FOIA.Flow.Runtime
{
    public sealed class EdgeFlowStateStore : MonoBehaviour
    {
        [SerializeField] private List<EdgeBlockDefinition> defaultBlocks = new();

        private readonly Dictionary<string, List<EdgeBlockDefinition>> blocksByEdgeId = new();

        public void InstallBlock(string edgeId, EdgeBlockDefinition block)
        {
            if (string.IsNullOrEmpty(edgeId) || block == null)
            {
                return;
            }

            if (!blocksByEdgeId.TryGetValue(edgeId, out List<EdgeBlockDefinition> blocks))
            {
                blocks = new List<EdgeBlockDefinition>();
                blocksByEdgeId.Add(edgeId, blocks);
            }

            if (!blocks.Contains(block))
            {
                blocks.Add(block);
            }
        }

        public IReadOnlyList<EdgeBlockDefinition> GetBlocks(string edgeId)
        {
            if (string.IsNullOrEmpty(edgeId) || !blocksByEdgeId.TryGetValue(edgeId, out List<EdgeBlockDefinition> blocks))
            {
                return defaultBlocks;
            }

            return blocks;
        }
    }
}
