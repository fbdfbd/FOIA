using OneMoreSpoon.Game.Components;
using OneMoreSpoon.Game.Core;
using OneMoreSpoon.View.Common;
using OneMoreSpoon.View.Factories;
using OneMoreSpoon.View.Flows;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;
using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Presenter
{
    public sealed class FlowViewSyncSystem : ITickable
    {
        private readonly GameWorld world;
        private readonly FlowViewFactory flowViewFactory;
        private readonly ViewRegistry viewRegistry;
        private readonly List<GameEntityId> removeBuffer = new();

        public FlowViewSyncSystem(
            GameWorld world,
            FlowViewFactory flowViewFactory,
            ViewRegistry viewRegistry)
        {
            this.world = world;
            this.flowViewFactory = flowViewFactory;
            this.viewRegistry = viewRegistry;
        }

        public void Tick()
        {
            CreateMissingViews();
            RenderActiveViews();
            RemoveStaleViews();
        }

        private void CreateMissingViews()
        {
            foreach (var pair in world.Flows)
            {
                if (pair.Value.State == FlowState.Consumed)
                    continue;

                if (viewRegistry.TryGetView(pair.Key, out _))
                    continue;

                flowViewFactory.Create(pair.Key);
            }
        }

        private void RemoveStaleViews()
        {
            removeBuffer.Clear();

            foreach (var pair in world.Flows)
            {
                if (pair.Value.State == FlowState.Consumed)
                    removeBuffer.Add(pair.Key);
            }

            foreach (var flowEntityId in removeBuffer)
                RemoveView(flowEntityId);
        }

        private void RemoveView(GameEntityId flowEntityId)
        {
            if (!viewRegistry.TryGetView(flowEntityId, out var view))
                return;

            viewRegistry.Unregister(flowEntityId);
            Object.Destroy(view.gameObject);
        }

        private void RenderActiveViews()
        {
            foreach (var pair in world.Flows)
            {
                if (pair.Value.State == FlowState.Consumed)
                    continue;

                if (!viewRegistry.TryGetView(pair.Key, out EntityView entityView))
                    continue;

                if (entityView is FlowView flowView)
                    flowView.RenderFlow(pair.Value);
            }
        }
    }
}
