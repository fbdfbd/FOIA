using System;
using System.Text;
using FOIA.Core;
using R3;
using TMPro;
using UnityEngine;

namespace FOIA.Presentation
{
    public sealed class ProcessEffectTextView : MonoBehaviour
    {
        [SerializeField] private ProcessRunController processRunController;
        [SerializeField] private EffectTargetType targetType;
        [SerializeField] private string targetId;
        [SerializeField] private TMP_Text text;
        [SerializeField] private string emptyText = "";

        private IDisposable subscription;

        private void Awake()
        {
            if (processRunController == null)
                processRunController = FindFirstObjectByType<ProcessRunController>();

            if (text == null)
                text = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            if (text != null)
                text.text = emptyText;

            if (processRunController != null)
                subscription = processRunController.ProcessCompleted.Subscribe(UpdateText);
        }

        private void OnDisable()
        {
            subscription?.Dispose();
        }

        private void UpdateText(Runtime.ProcessResult result)
        {
            if (text == null)
                return;

            var builder = new StringBuilder();
            foreach (var effect in result.Effects)
            {
                if (effect.TargetType != targetType || effect.TargetId != targetId)
                    continue;

                if (builder.Length > 0)
                    builder.AppendLine();

                builder.Append(effect.Label);
                builder.Append(' ');
                builder.Append(effect.Amount > 0 ? "+" : "");
                builder.Append(effect.Amount);
            }

            text.text = builder.Length > 0 ? builder.ToString() : emptyText;
        }
    }
}
