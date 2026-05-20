using System;
using UnityEngine;

namespace FOIA.Core
{
    public enum ProcessTag
    {
        None = 0,
        Verified = 1,
        Classified = 2,
        PrivacyRisk = 3,
        LegalRisk = 4,
        Delay = 5,
        PublicSatisfaction = 6,
        AgencyFriction = 7,
        MissingEvidence = 8,
        PartialDisclosure = 9,
        AutomationNoise = 10
    }

    public enum ByproductType
    {
        None = 0,
        ProcessingKnowHow = 1,
        MissingClue = 2,
        PrivacyFragment = 3,
        LegalConcern = 4,
        ComplaintPressure = 5,
        AgencyFriction = 6
    }

    public enum ResultGrade
    {
        Failed = 0,
        Risky = 1,
        Acceptable = 2,
        Excellent = 3
    }

    public enum EffectTargetType
    {
        Complaint = 0,
        Node = 1,
        Edge = 2,
        Agency = 3,
        Staff = 4
    }

    public enum ProcessEffectType
    {
        PrivacyExposure = 0,
        WeakLegalBasis = 1,
        ProcessingDelay = 2,
        MissingRecord = 3,
        AutomationMistake = 4,
        AgencyResistance = 5,
        StaffStress = 6,
        StaffConfidence = 7,
        PublicTrust = 8
    }

    [Serializable]
    public struct TagModifier
    {
        public ProcessTag tag;
        public int amount;
    }

    [Serializable]
    public struct ByproductAmount
    {
        public ByproductType type;
        [Min(1)] public int amount;
    }

    [Serializable]
    public readonly struct ProcessEffect
    {
        public ProcessEffect(EffectTargetType targetType, string targetId, ProcessEffectType effectType, int amount, string label)
        {
            TargetType = targetType;
            TargetId = targetId;
            EffectType = effectType;
            Amount = amount;
            Label = label;
        }

        public EffectTargetType TargetType { get; }
        public string TargetId { get; }
        public ProcessEffectType EffectType { get; }
        public int Amount { get; }
        public string Label { get; }
    }
}
