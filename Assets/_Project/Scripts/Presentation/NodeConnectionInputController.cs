using System;
using FOIA.Core;
using FOIA.Input;
using R3;
using UnityEngine;

namespace FOIA.Presentation
{
    public readonly struct NodeConnectionRequest
    {
        public NodeConnectionRequest(NodeId from, NodeId to)
        {
            From = from;
            To = to;
        }

        public NodeId From { get; }
        public NodeId To { get; }
    }

    public sealed class NodeConnectionInputController : MonoBehaviour
    {
        [SerializeField] private BoardPointerInput pointerInput;

        private readonly Subject<NodeConnectionRequest> connectionRequested = new();
        private IDisposable downSubscription;
        private IDisposable upSubscription;
        private NodeView connectionStart;

        public Observable<NodeConnectionRequest> ConnectionRequested => connectionRequested;

        private void Awake()
        {
            if (pointerInput == null)
                pointerInput = FindFirstObjectByType<BoardPointerInput>();
        }

        private void OnEnable()
        {
            if (pointerInput == null)
                return;

            downSubscription = pointerInput.PointerDown.Subscribe(OnPointerDown);
            upSubscription = pointerInput.PointerUp.Subscribe(OnPointerUp);
        }

        private void OnDisable()
        {
            downSubscription?.Dispose();
            upSubscription?.Dispose();
        }

        private void OnDestroy()
        {
            connectionRequested.Dispose();
        }

        private void OnPointerDown(BoardPointerEvent pointerEvent)
        {
            connectionStart = pointerEvent.Target as NodeView;
        }

        private void OnPointerUp(BoardPointerEvent pointerEvent)
        {
            if (connectionStart == null)
                return;

            var start = connectionStart;
            connectionStart = null;

            if (pointerEvent.Target is not NodeView end
                || start == end
                || start.Definition == null
                || end.Definition == null)
            {
                return;
            }

            connectionRequested.OnNext(new NodeConnectionRequest(start.NodeId, end.NodeId));
        }
    }
}
