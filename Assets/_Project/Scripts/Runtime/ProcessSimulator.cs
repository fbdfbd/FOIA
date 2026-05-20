using System.Collections.Generic;
using FOIA.Core;
using FOIA.Definitions;

namespace FOIA.Runtime
{
    public sealed class ProcessSimulator
    {
        private const string DefaultStaffTarget = "\uB2F4\uB2F9\uC790";
        private const string DefaultAgencyTarget = "\uBCF4\uC720\uBD80\uC11C";
        private const string WholeProcessTarget = "\uC804\uCCB4 \uC808\uCC28";
        private const string AgencyQueryTarget = "\uAE30\uAD00 \uC870\uD68C";
        private const string RequirementNodeTarget = "\uCCAD\uAD6C\uC694\uAC74 \uAC80\uD1A0";

        private readonly DefinitionCatalog catalog;

        public ProcessSimulator(DefinitionCatalog catalog)
        {
            this.catalog = catalog;
        }

        public ProcessResult Run(
            ComplaintTypeId complaintTypeId,
            IReadOnlyList<EdgeId> path,
            ProcessBoardState boardState,
            StaffDefinition staff = null,
            AgencyDefinition agency = null)
        {
            var complaintDefinition = catalog.GetComplaint(complaintTypeId);
            var complaint = new ComplaintInstance(complaintTypeId);
            var effects = new List<ProcessEffect>();

            foreach (var modifier in complaintDefinition.InitialTags)
                complaint.AddTag(modifier.tag, modifier.amount);

            ApplyStaff(staff, complaint, effects);
            ApplyAgency(agency, complaint, effects);

            foreach (var edgeId in path)
                ApplyEdge(edgeId, boardState, complaint, effects, agency);

            AddFinalEffects(complaint, effects, staff, agency);

            return new ProcessResult(Evaluate(complaint), complaint.Tags, complaint.Byproducts, effects);
        }

        private static void ApplyStaff(StaffDefinition staff, ComplaintInstance complaint, List<ProcessEffect> effects)
        {
            if (staff == null)
                return;

            foreach (var modifier in staff.TagModifiers)
                complaint.AddTag(modifier.tag, modifier.amount);

            var target = staff.DisplayName;
            switch (staff.Profile)
            {
                case StaffProfile.FastStressful:
                    effects.Add(new ProcessEffect(EffectTargetType.Staff, target, ProcessEffectType.StaffStress, 2, "\uBE60\uB978 \uCC98\uB9AC \uD53C\uB85C"));
                    effects.Add(new ProcessEffect(EffectTargetType.Edge, WholeProcessTarget, ProcessEffectType.ProcessingDelay, -2, "\uCC98\uB9AC \uB2E8\uCD95"));
                    break;
                case StaffProfile.SlowStable:
                    effects.Add(new ProcessEffect(EffectTargetType.Staff, target, ProcessEffectType.StaffStress, -1, "\uC548\uC815 \uCC98\uB9AC"));
                    effects.Add(new ProcessEffect(EffectTargetType.Edge, WholeProcessTarget, ProcessEffectType.ProcessingDelay, 1, "\uC2E0\uC911\uD55C \uC9C0\uC5F0"));
                    break;
                case StaffProfile.ProblemFinder:
                    effects.Add(new ProcessEffect(EffectTargetType.Staff, target, ProcessEffectType.StaffConfidence, 2, "\uBB38\uC81C \uBC1C\uACAC"));
                    effects.Add(new ProcessEffect(EffectTargetType.Node, RequirementNodeTarget, ProcessEffectType.MissingRecord, -2, "\uB204\uB77D \uBC1C\uACAC"));
                    break;
            }
        }

        private static void ApplyAgency(AgencyDefinition agency, ComplaintInstance complaint, List<ProcessEffect> effects)
        {
            if (agency == null)
                return;

            foreach (var modifier in agency.TagModifiers)
                complaint.AddTag(modifier.tag, modifier.amount);

            var target = agency.DisplayName;
            switch (agency.Profile)
            {
                case AgencyProfile.CooperativeSlow:
                    effects.Add(new ProcessEffect(EffectTargetType.Agency, target, ProcessEffectType.AgencyResistance, -1, "\uD611\uC870\uC801 \uD0DC\uB3C4"));
                    effects.Add(new ProcessEffect(EffectTargetType.Edge, AgencyQueryTarget, ProcessEffectType.ProcessingDelay, 2, "\uB290\uB9B0 \uD68C\uC2E0"));
                    break;
                case AgencyProfile.FastSensitive:
                    effects.Add(new ProcessEffect(EffectTargetType.Agency, target, ProcessEffectType.AgencyResistance, 1, "\uAD00\uACC4 \uBBFC\uAC10"));
                    effects.Add(new ProcessEffect(EffectTargetType.Edge, AgencyQueryTarget, ProcessEffectType.ProcessingDelay, -2, "\uBE60\uB978 \uD68C\uC2E0"));
                    break;
            }
        }

        private void ApplyEdge(
            EdgeId edgeId,
            ProcessBoardState boardState,
            ComplaintInstance complaint,
            List<ProcessEffect> effects,
            AgencyDefinition agency)
        {
            var edge = catalog.GetEdge(edgeId);
            foreach (var modifier in edge.BaseModifiers)
            {
                complaint.AddTag(modifier.tag, modifier.amount);
                AddImmediateEffect(edgeId.Value, modifier, effects, agency);
            }

            if (!boardState.TryGetEquippedBlock(edgeId, out var blockId))
                return;

            var block = catalog.GetEdgeBlock(blockId);
            foreach (var modifier in block.TagModifiers)
            {
                complaint.AddTag(modifier.tag, modifier.amount);
                AddImmediateEffect(edgeId.Value, modifier, effects, agency);
            }

            foreach (var byproduct in block.Byproducts)
                complaint.AddByproduct(byproduct.type, byproduct.amount);
        }

        private static void AddImmediateEffect(string edgeId, TagModifier modifier, List<ProcessEffect> effects, AgencyDefinition agency)
        {
            var agencyTarget = agency != null ? agency.DisplayName : DefaultAgencyTarget;

            switch (modifier.tag)
            {
                case ProcessTag.Delay:
                    effects.Add(new ProcessEffect(EffectTargetType.Edge, edgeId, ProcessEffectType.ProcessingDelay, modifier.amount, "\uCC98\uB9AC \uC9C0\uC5F0"));
                    effects.Add(new ProcessEffect(EffectTargetType.Staff, DefaultStaffTarget, ProcessEffectType.StaffStress, modifier.amount, "\uB2F4\uB2F9\uC790 \uD53C\uB85C"));
                    break;
                case ProcessTag.AgencyFriction:
                    var sensitivity = agency != null ? agency.RelationshipSensitivity : 0;
                    effects.Add(new ProcessEffect(EffectTargetType.Agency, agencyTarget, ProcessEffectType.AgencyResistance, modifier.amount + sensitivity, "\uAE30\uAD00 \uBC18\uBC1C"));
                    break;
                case ProcessTag.AutomationNoise:
                    effects.Add(new ProcessEffect(EffectTargetType.Edge, edgeId, ProcessEffectType.AutomationMistake, modifier.amount, "\uC790\uB3D9\uCC98\uB9AC \uC624\uB958"));
                    break;
            }
        }

        private static void AddFinalEffects(ComplaintInstance complaint, List<ProcessEffect> effects, StaffDefinition staff, AgencyDefinition agency)
        {
            var staffTarget = staff != null ? staff.DisplayName : DefaultStaffTarget;
            var agencyTarget = agency != null ? agency.DisplayName : DefaultAgencyTarget;

            AddIfNonZero(effects, EffectTargetType.Complaint, complaint.TypeId.Value, ProcessEffectType.PrivacyExposure, complaint.GetTag(ProcessTag.PrivacyRisk), "\uAC1C\uC778\uC815\uBCF4 \uC704\uD5D8");
            AddIfNonZero(effects, EffectTargetType.Complaint, complaint.TypeId.Value, ProcessEffectType.WeakLegalBasis, complaint.GetTag(ProcessTag.LegalRisk), "\uBC95\uC801 \uADFC\uAC70 \uBD80\uC2E4");
            AddIfNonZero(effects, EffectTargetType.Node, RequirementNodeTarget, ProcessEffectType.MissingRecord, complaint.GetTag(ProcessTag.MissingEvidence), "\uC790\uB8CC \uB204\uB77D");
            AddIfNonZero(effects, EffectTargetType.Agency, agencyTarget, ProcessEffectType.AgencyResistance, complaint.GetTag(ProcessTag.AgencyFriction), "\uB204\uC801 \uAE30\uAD00 \uB9C8\uCC30");

            var goodHandling = complaint.GetTag(ProcessTag.Verified)
                + complaint.GetTag(ProcessTag.Classified)
                + complaint.GetTag(ProcessTag.PartialDisclosure);
            AddIfNonZero(effects, EffectTargetType.Staff, staffTarget, ProcessEffectType.StaffConfidence, goodHandling, "\uCC98\uB9AC \uC219\uB828");

            var trust = complaint.GetTag(ProcessTag.PublicSatisfaction)
                - complaint.GetTag(ProcessTag.LegalRisk)
                - complaint.GetTag(ProcessTag.PrivacyRisk);
            AddIfNonZero(effects, EffectTargetType.Complaint, complaint.TypeId.Value, ProcessEffectType.PublicTrust, trust, "\uCCAD\uAD6C\uC778 \uC2E0\uB8B0");
        }

        private static void AddIfNonZero(List<ProcessEffect> effects, EffectTargetType targetType, string targetId, ProcessEffectType effectType, int amount, string label)
        {
            if (amount != 0)
                effects.Add(new ProcessEffect(targetType, targetId, effectType, amount, label));
        }

        private static ResultGrade Evaluate(ComplaintInstance complaint)
        {
            var risk = complaint.GetTag(ProcessTag.PrivacyRisk)
                + complaint.GetTag(ProcessTag.LegalRisk)
                + complaint.GetTag(ProcessTag.MissingEvidence)
                + complaint.GetTag(ProcessTag.AutomationNoise);
            var quality = complaint.GetTag(ProcessTag.Verified)
                + complaint.GetTag(ProcessTag.Classified)
                + complaint.GetTag(ProcessTag.PartialDisclosure)
                + complaint.GetTag(ProcessTag.PublicSatisfaction);
            var delay = complaint.GetTag(ProcessTag.Delay);

            if (risk >= quality + 3)
                return ResultGrade.Failed;

            if (risk > quality || delay >= 5)
                return ResultGrade.Risky;

            if (quality >= risk + 4)
                return ResultGrade.Excellent;

            return ResultGrade.Acceptable;
        }
    }
}
