using OneMoreSpoon.Game.Components;
using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Definitions;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Systems
{
    public sealed class EdgeBlockEquipSystem
    {
        private readonly GameWorld world;
        private readonly SubstanceStackSystem stackSystem;
        private readonly EdgeBlockReturnSystem edgeBlockReturnSystem;
        private readonly SubstanceDefinitionRegistry definitionRegistry;

        public EdgeBlockEquipSystem(
            GameWorld world,
            SubstanceStackSystem stackSystem,
            EdgeBlockReturnSystem edgeBlockReturnSystem,
            SubstanceDefinitionRegistry definitionRegistry)
        {
            this.world = world;
            this.stackSystem = stackSystem;
            this.edgeBlockReturnSystem = edgeBlockReturnSystem;
            this.definitionRegistry = definitionRegistry;
        }

        public bool TryEquip(GameEntityId edgeId, GameEntityId stackId)
        {
            if (!world.Edges.ContainsKey(edgeId))
            {
                Debug.LogWarning($"[EdgeBlockEquip] Failed edge={edgeId} stack={stackId} reason=EdgeNotFound");
                return false;
            }

            if (!world.SubstanceStacks.TryGetValue(stackId, out var stack))
            {
                Debug.LogWarning($"[EdgeBlockEquip] Failed edge={edgeId} stack={stackId} reason=StackNotFound");
                return false;
            }

            if (!definitionRegistry.TryGet(stack.SubstanceId, out var definition))
            {
                Debug.LogWarning($"[EdgeBlockEquip] Failed edge={edgeId} stack={stackId} substance={stack.SubstanceId} reason=SubstanceDefinitionNotFound");
                return false;
            }

            if (definition.Kind != SubstanceKind.EdgeBlock)
            {
                Debug.LogWarning($"[EdgeBlockEquip] Failed edge={edgeId} stack={stackId} substance={stack.SubstanceId} reason=SubstanceIsNotEdgeBlock kind={definition.Kind}");
                return false;
            }

            if (!world.EdgeBlockSlots.TryGetValue(edgeId, out var slot))
            {
                Debug.LogWarning($"[EdgeBlockEquip] Failed edge={edgeId} stack={stackId} substance={stack.SubstanceId} reason=SlotNotFound");
                return false;
            }

            if (!stackSystem.CanConsume(stackId))
            {
                Debug.LogWarning($"[EdgeBlockEquip] Failed edge={edgeId} stack={stackId} substance={stack.SubstanceId} reason=StackEmpty");
                return false;
            }

            if (slot.HasBlock)
            {
                Debug.Log($"[EdgeBlockEquip] Replaced edge={edgeId} old={slot.EquippedSubstanceId} next={stack.SubstanceId}");

                if (!edgeBlockReturnSystem.TryReturn(edgeId))
                {
                    Debug.LogWarning($"[EdgeBlockEquip] Failed edge={edgeId} stack={stackId} reason=ReturnOldBlockFailed");
                    return false;
                }
            }

            if (!stackSystem.TryConsume(stackId))
            {
                Debug.LogWarning($"[EdgeBlockEquip] Failed edge={edgeId} stack={stackId} substance={stack.SubstanceId} reason=ConsumeFailed");
                return false;
            }

            slot.EquippedSubstanceId = stack.SubstanceId;
            world.EdgeBlockSlots[edgeId] = slot;

            if (stackSystem.IsEmpty(stackId))
                stackSystem.Remove(stackId);

            Debug.Log($"[EdgeBlockEquip] Succeeded edge={edgeId} stack={stackId} substance={stack.SubstanceId}");
            return true;
        }
    }
}
