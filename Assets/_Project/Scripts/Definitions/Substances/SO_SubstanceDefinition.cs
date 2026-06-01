using System.Collections.Generic;
using UnityEngine;

namespace OneMoreSpoon.Game.Definitions
{
    [CreateAssetMenu(
        fileName = "SO_SubstanceDefinition",
        menuName = "OneMoreSpoon/Definitions/Substance Definition")]
    public sealed class SO_SubstanceDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string substanceId;
        [SerializeField] private string displayName;
        [SerializeField] private Sprite image;

        [Header("Classification")]
        [SerializeField] private SubstanceKind kind;
        [SerializeField] private List<string> baseTags = new();

        [Header("Economy")]
        [SerializeField] private int baseValue;

        [Header("Edge Block Effect")]
        [SerializeField] private List<string> addedTags = new();
        [SerializeField] private float durationMultiplier = 1f;
        [SerializeField] private float valueMultiplier = 1f;

        public string SubstanceId => substanceId;
        public string DisplayName => displayName;
        public Sprite Image => image;
        public SubstanceKind Kind => kind;
        public IReadOnlyList<string> BaseTags => baseTags;
        public int BaseValue => baseValue;
        public IReadOnlyList<string> AddedTags => addedTags;
        public float DurationMultiplier => durationMultiplier;
        public float ValueMultiplier => valueMultiplier;
    }
}
