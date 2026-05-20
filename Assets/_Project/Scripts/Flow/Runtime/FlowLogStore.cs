using System;
using System.Collections.Generic;
using UnityEngine;

namespace FOIA.Flow.Runtime
{
    public sealed class FlowLogStore : MonoBehaviour
    {
        private readonly List<string> messages = new();

        public event Action LogChanged;
        public IReadOnlyList<string> Messages => messages;

        public void Add(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            messages.Add(message);
            LogChanged?.Invoke();
        }

        public void Clear()
        {
            messages.Clear();
            LogChanged?.Invoke();
        }
    }
}
