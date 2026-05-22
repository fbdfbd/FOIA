using GameEntityId = OneMoreSpoon.Game.Core.EntityId;

namespace OneMoreSpoon.Game.Components
{
    public struct FlowComponent
    {
        public GameEntityId CurrentNodeId;
        public GameEntityId CurrentEdgeId;
        public float Progress;
        public FlowState State;

        public FlowComponent(GameEntityId startNodeId)
        {
            CurrentNodeId = startNodeId;
            CurrentEdgeId = GameEntityId.Invalid;
            Progress = 0f;
            State = FlowState.WaitingAtNode;
        }

        public void BeginEdge(GameEntityId edgeId)
        {
            CurrentEdgeId = edgeId;
            Progress = 0f;
            State = FlowState.MovingOnEdge;
        }

        public void ArriveAtNode(GameEntityId nodeId)
        {
            CurrentNodeId = nodeId;
            CurrentEdgeId = GameEntityId.Invalid;
            Progress = 0f;
            State = FlowState.WaitingAtNode;
        }

        public void ArriveAtOutput(GameEntityId nodeId)
        {
            CurrentNodeId = nodeId;
            CurrentEdgeId = GameEntityId.Invalid;
            Progress = 1f;
            State = FlowState.ArrivedAtOutput;
        }

        public void Consume()
        {
            CurrentEdgeId = GameEntityId.Invalid;
            Progress = 1f;
            State = FlowState.Consumed;
        }
    }
}
