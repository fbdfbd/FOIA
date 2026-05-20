using FOIA.Core.Tags;
using System.Collections.Generic;
using UnityEngine;

namespace FOIA.Requests.Definitions
{
    [CreateAssetMenu(fileName = "SO_RequestDefinition", menuName = "FOIA/Requests/SO_RequestDefinition")]
    public sealed class SO_RequestDefinition : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private string _title;

        [SerializeField] private int _aggression;
        [SerializeField] private int _urgency;
        [SerializeField] private int _mediaRisk;
        [SerializeField] private int _pendingGraceHours = 3;

        [SerializeField] private GameTag[] _tags;
        [SerializeField] private GameTag[] _targetAgencyTags;

        public string Id => _id;
        public string Title => _title;
        public int Aggression => _aggression;
        public int Urgency => _urgency;
        public int MediaRisk => _mediaRisk;
        public int PendingGraceHours => _pendingGraceHours;
        public IReadOnlyList<GameTag> Tags => _tags;
        public IReadOnlyList<GameTag> TargetAgencyTags => _targetAgencyTags;
    }
}
