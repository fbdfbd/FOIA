using System.Collections.Generic;
using UnityEngine;

namespace OneMoreSpoon.App.Messaging
{
    public sealed class ToastMessageQueue
    {
        private readonly Queue<string> messages = new();

        public int Count => messages.Count;

        public void Enqueue(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            messages.Enqueue(message);
            Debug.Log($"[Toast] {message}");
        }

        public bool TryDequeue(out string message)
        {
            if (messages.Count <= 0)
            {
                message = string.Empty;
                return false;
            }

            message = messages.Dequeue();
            return true;
        }
    }
}
