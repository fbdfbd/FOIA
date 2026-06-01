using System;

namespace OneMoreSpoon.App.Messaging
{
    public static class NodeDiscoveryEvents
    {
        public static event Action<string> Discovered;

        public static void RaiseDiscovered(string nodeDefinitionId)
        {
            if (string.IsNullOrWhiteSpace(nodeDefinitionId))
                return;

            Discovered?.Invoke(nodeDefinitionId);
        }
    }
}
