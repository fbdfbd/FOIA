using OneMoreSpoon.Game.Core;
using OneMoreSpoon.Game.Components;
using OneMoreSpoon.Game.Definitions;
using OneMoreSpoon.View.Substances;
using System.Collections.Generic;
using UnityEngine;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Systems
{
    public sealed class SubstanceStackSystem
    {
        private readonly GameWorld world;
        private readonly PlayAreaBoundsSystem playAreaBounds;

        public SubstanceStackSystem(GameWorld world, PlayAreaBoundsSystem playAreaBounds)
        {
            this.world = world;
            this.playAreaBounds = playAreaBounds;
        }

        public bool TryConsume(GameEntityId stackId)
        {
            if (!world.SubstanceStacks.TryGetValue(stackId, out var stack))
                return false;

            if (stack.IsInfinite)
                return true;

            if (stack.Amount <= 0)
                return false;

            stack.Amount--;
            world.SubstanceStacks[stackId] = stack;

            return true;
        }

        public bool CanConsume(GameEntityId stackId)
        {
            if (!world.SubstanceStacks.TryGetValue(stackId, out var stack))
                return false;

            return stack.IsInfinite || stack.Amount > 0;
        }

        public bool IsEmpty(GameEntityId stackId)
        {
            return world.SubstanceStacks.TryGetValue(stackId, out var stack) && stack.IsEmpty;
        }

        public void Remove(GameEntityId stackId)
        {
            world.SubstanceStacks.Remove(stackId);
            world.Positions.Remove(stackId);
        }

        public bool TryMove(GameEntityId stackId, Vector2 position)
        {
            if (!world.SubstanceStacks.ContainsKey(stackId))
                return false;

            Vector2 clampedPosition = playAreaBounds.Clamp(position);
            world.Positions[stackId] = new PositionComponent(clampedPosition);
            return true;
        }
    }

    public enum SubstanceDockKind
    {
        Dish,
        EdgeBlock,
        TraitShard
    }

    public sealed class SubstanceDockDepthState
    {
        private readonly Dictionary<GameEntityId, float> depthsByStackId = new();

        public float GetDepth(GameEntityId stackId)
        {
            return depthsByStackId.TryGetValue(stackId, out float depth)
                ? depth
                : 0f;
        }

        public void SetDepth(GameEntityId stackId, float depth)
        {
            depthsByStackId[stackId] = depth;
        }

        public void ClearDepth(GameEntityId stackId)
        {
            depthsByStackId.Remove(stackId);
        }
    }

    public sealed class SubstanceDockLayoutSettings
    {
        public float HorizontalSpacing { get; } = 1.05f;
        public float VerticalSpacing { get; } = 1.05f;
        public float HorizontalPadding { get; } = 0.55f;
        public float VerticalPadding { get; } = 0.55f;
        public float DockedStackZStep { get; } = -0.01f;
    }

    public readonly struct SubstanceDockArea
    {
        public readonly SubstanceDockKind Kind;
        public readonly string Label;
        public readonly Bounds Bounds;

        public SubstanceDockArea(SubstanceDockKind kind, string label, Bounds bounds)
        {
            Kind = kind;
            Label = label;
            Bounds = bounds;
        }

        public bool Contains(Vector2 position)
        {
            return Bounds.Contains(new Vector3(position.x, position.y, Bounds.center.z));
        }
    }

    public sealed class SubstanceDockSystem
    {
        private readonly GameWorld world;
        private readonly SubstanceStackSystem stackSystem;
        private readonly PlayAreaBoundsSystem playAreaBounds;
        private readonly SubstanceDefinitionRegistry definitionRegistry;
        private readonly SubstanceDockLayoutSettings settings;
        private readonly SubstanceDockDepthState depthState;
        private readonly Dictionary<GameEntityId, SubstanceDockKind> dockedStacks = new();
        private readonly HashSet<GameEntityId> knownStacks = new();
        private readonly HashSet<GameEntityId> draggingStacks = new();
        private readonly List<SubstanceDockArea> areaBuffer = new();
        private readonly List<GameEntityId> stackBuffer = new();
        private readonly List<GameEntityId> staleBuffer = new();
        private IReadOnlyList<SubstanceDockArea> cachedAreas;

        public SubstanceDockSystem(
            GameWorld world,
            SubstanceStackSystem stackSystem,
            PlayAreaBoundsSystem playAreaBounds,
            SubstanceDefinitionRegistry definitionRegistry,
            SubstanceDockLayoutSettings settings,
            SubstanceDockDepthState depthState)
        {
            this.world = world;
            this.stackSystem = stackSystem;
            this.playAreaBounds = playAreaBounds;
            this.definitionRegistry = definitionRegistry;
            this.settings = settings;
            this.depthState = depthState;
        }

        public IReadOnlyList<SubstanceDockArea> Areas
        {
            get
            {
                if (cachedAreas == null)
                    cachedAreas = FindSceneAreas();

                return cachedAreas;
            }
        }

        public void SyncNewEligibleStacks()
        {
            RemoveStaleStacks();

            foreach (var pair in world.SubstanceStacks)
            {
                GameEntityId stackId = pair.Key;

                if (knownStacks.Contains(stackId))
                    continue;

                knownStacks.Add(stackId);

                if (!TryGetDockKind(pair.Value.SubstanceId, out SubstanceDockKind dockKind))
                    continue;

                DockInternal(stackId, dockKind);
            }

            RearrangeAll();
        }

        public void BeginDrag(GameEntityId stackId)
        {
            knownStacks.Add(stackId);
            draggingStacks.Add(stackId);

            if (!dockedStacks.TryGetValue(stackId, out SubstanceDockKind dockKind))
                return;

            dockedStacks.Remove(stackId);
            depthState.ClearDepth(stackId);
            Rearrange(dockKind);
        }

        public void EndDrag(GameEntityId stackId)
        {
            draggingStacks.Remove(stackId);
        }

        public bool TryDock(GameEntityId stackId, SubstanceDockKind dockKind)
        {
            if (!world.SubstanceStacks.TryGetValue(stackId, out var stack))
                return false;

            if (!TryGetDockKind(stack.SubstanceId, out SubstanceDockKind expectedDockKind))
                return false;

            if (expectedDockKind != dockKind)
                return false;

            knownStacks.Add(stackId);
            draggingStacks.Remove(stackId);
            DockInternal(stackId, dockKind);
            Rearrange(dockKind);
            return true;
        }

        public void MarkFree(GameEntityId stackId)
        {
            knownStacks.Add(stackId);
            draggingStacks.Remove(stackId);

            if (dockedStacks.TryGetValue(stackId, out SubstanceDockKind dockKind))
            {
                dockedStacks.Remove(stackId);
                Rearrange(dockKind);
            }

            depthState.ClearDepth(stackId);
        }

        public bool TryGetDockAt(Vector2 position, out SubstanceDockKind dockKind)
        {
            foreach (var area in Areas)
            {
                if (!area.Contains(position))
                    continue;

                dockKind = area.Kind;
                return true;
            }

            dockKind = default;
            return false;
        }

        public bool IsDocked(GameEntityId stackId)
        {
            return dockedStacks.ContainsKey(stackId);
        }

        private void DockInternal(GameEntityId stackId, SubstanceDockKind dockKind)
        {
            dockedStacks[stackId] = dockKind;
        }

        private void RearrangeAll()
        {
            foreach (var area in Areas)
                Rearrange(area.Kind);
        }

        private void Rearrange(SubstanceDockKind dockKind)
        {
            CollectDockedStacks(dockKind);

            if (stackBuffer.Count <= 0)
                return;

            SubstanceDockArea area = GetArea(dockKind);

            for (int i = 0; i < stackBuffer.Count; i++)
            {
                GameEntityId stackId = stackBuffer[i];

                if (draggingStacks.Contains(stackId))
                    continue;

                stackSystem.TryMove(stackId, GetSlotPosition(area.Bounds, i, stackBuffer.Count));
                depthState.SetDepth(stackId, settings.DockedStackZStep * i);
            }
        }

        private void CollectDockedStacks(SubstanceDockKind dockKind)
        {
            stackBuffer.Clear();

            foreach (var pair in dockedStacks)
            {
                if (pair.Value == dockKind && world.SubstanceStacks.ContainsKey(pair.Key))
                    stackBuffer.Add(pair.Key);
            }

            stackBuffer.Sort((a, b) => a.Value.CompareTo(b.Value));
        }

        private Vector2 GetSlotPosition(Bounds bounds, int index, int count)
        {
            float usableWidth = Mathf.Max(0.1f, bounds.size.x - settings.HorizontalPadding * 2f);
            int columns = Mathf.Max(1, Mathf.FloorToInt(usableWidth / settings.HorizontalSpacing) + 1);
            int rows = Mathf.Max(1, Mathf.CeilToInt(count / (float)columns));

            float horizontalSpacing = columns <= 1
                ? 0f
                : Mathf.Min(settings.HorizontalSpacing, usableWidth / (columns - 1));

            float usableHeight = Mathf.Max(0.1f, bounds.size.y - settings.VerticalPadding * 2f);
            float verticalSpacing = rows <= 1
                ? 0f
                : Mathf.Min(settings.VerticalSpacing, usableHeight / (rows - 1));

            int column = index % columns;
            int row = index / columns;

            return new Vector2(
                bounds.min.x + settings.HorizontalPadding + horizontalSpacing * column,
                bounds.max.y - settings.VerticalPadding - verticalSpacing * row);
        }

        private SubstanceDockArea GetArea(SubstanceDockKind dockKind)
        {
            foreach (var area in Areas)
                if (area.Kind == dockKind)
                    return area;

            return default;
        }

        private IReadOnlyList<SubstanceDockArea> FindSceneAreas()
        {
            areaBuffer.Clear();

            foreach (var view in Object.FindObjectsByType<SubstanceDockAreaView>(FindObjectsSortMode.None))
            {
                if (view == null || !view.isActiveAndEnabled)
                    continue;

                areaBuffer.Add(view.ToArea());
            }

            areaBuffer.Sort((a, b) => a.Kind.CompareTo(b.Kind));
            return areaBuffer.ToArray();
        }

        private bool TryGetDockKind(string substanceId, out SubstanceDockKind dockKind)
        {
            if (!definitionRegistry.TryGet(substanceId, out SO_SubstanceDefinition definition))
            {
                dockKind = default;
                return false;
            }

            switch (definition.Kind)
            {
                case SubstanceKind.Dish:
                case SubstanceKind.FinalDish:
                    dockKind = SubstanceDockKind.Dish;
                    return true;

                case SubstanceKind.EdgeBlock:
                    dockKind = SubstanceDockKind.EdgeBlock;
                    return true;

                case SubstanceKind.TraitShard:
                    dockKind = SubstanceDockKind.TraitShard;
                    return true;

                default:
                    dockKind = default;
                    return false;
            }
        }

        private void RemoveStaleStacks()
        {
            staleBuffer.Clear();

            foreach (var stackId in knownStacks)
                if (!world.SubstanceStacks.ContainsKey(stackId))
                    staleBuffer.Add(stackId);

            foreach (var stackId in staleBuffer)
            {
                knownStacks.Remove(stackId);
                draggingStacks.Remove(stackId);
                dockedStacks.Remove(stackId);
                depthState.ClearDepth(stackId);
            }
        }
    }
}
