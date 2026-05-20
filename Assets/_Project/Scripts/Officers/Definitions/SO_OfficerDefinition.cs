using FOIA.Core.Tags;
using System.Collections.Generic;
using UnityEngine;

namespace FOIA.Officers.Definitions
{
    [CreateAssetMenu(fileName = "SO_OfficerDefinition", menuName = "FOIA/Officers/SO_OfficerDefinition")]
    public sealed class SO_OfficerDefinition : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _displayName;
        [SerializeField] private string _role;

        [SerializeField] private int _maxStress = 100;
        [SerializeField] private int _stressResistance;
        [SerializeField] private GameTag[] _tags;

        public string Id => _id;
        public string DisplayName => _displayName;
        public string Role => _role;
        public int MaxStress => _maxStress;
        public int StressResistance => _stressResistance;
        public IReadOnlyList<GameTag> Tags => _tags;
    }
}