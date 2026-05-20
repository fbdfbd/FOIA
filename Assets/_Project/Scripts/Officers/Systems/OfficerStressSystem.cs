using System;
using FOIA.Officers.Runtime;

namespace FOIA.Officers.Systems
{
    public sealed class OfficerStressSystem
    {
        public void AddStress(OfficerRuntime officer, int amount)
        {
            if (officer == null)
            {
                throw new ArgumentNullException(nameof(officer));
            }

            if (amount <= 0 || officer.Status == OfficerStatus.Resigned)
            {
                return;
            }

            int actualAmount = Math.Max(0, amount - officer.Definition.StressResistance);
            int nextStress = Math.Min(officer.Definition.MaxStress, officer.Stress + actualAmount);

            officer.SetStress(nextStress);
            officer.SetStatus(GetStatus(officer));
        }

        private static OfficerStatus GetStatus(OfficerRuntime officer)
        {
            if (officer.Stress >= officer.Definition.MaxStress)
            {
                return OfficerStatus.Resigned;
            }

            if (officer.Stress >= officer.Definition.MaxStress * 0.8f)
            {
                return OfficerStatus.BurnedOut;
            }

            if (officer.Stress >= officer.Definition.MaxStress * 0.5f)
            {
                return OfficerStatus.Stressed;
            }

            return OfficerStatus.Available;
        }
    }
}
