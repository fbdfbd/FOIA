using System.Text;
using FOIA.Flow.Runtime;
using FOIA.Graph.Runtime;
using TMPro;
using UnityEngine;

namespace FOIA.Flow.Presentation
{
    public sealed class FlowLogView : MonoBehaviour
    {
        [SerializeField] private FlowLogStore logStore;
        [SerializeField] private TMP_Text label;

        private readonly StringBuilder builder = new();

        private void Awake()
        {
            if (logStore == null)
            {
                logStore = GraphSceneLookup.FindFirst<FlowLogStore>();
            }

            if (label == null)
            {
                label = GetComponentInChildren<TMP_Text>();
            }
        }

        private void OnEnable()
        {
            if (logStore != null)
            {
                logStore.LogChanged += Refresh;
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (logStore != null)
            {
                logStore.LogChanged -= Refresh;
            }
        }

        private void Refresh()
        {
            if (label == null || logStore == null)
            {
                return;
            }

            builder.Clear();

            foreach (string message in logStore.Messages)
            {
                builder.Append("> ");
                builder.AppendLine(message);
            }

            label.text = builder.ToString();
        }
    }
}
