using FOIA.Flow.Definitions;

namespace FOIA.Flow.Runtime
{
    public sealed class StaffRuntime
    {
        public StaffRuntime(StaffDefinition definition)
        {
            Definition = definition;
            Stress = definition != null ? definition.StartStress : 0;
            IsActive = true;
        }

        public StaffDefinition Definition { get; }
        public int Stress { get; private set; }
        public bool IsActive { get; private set; }

        public void AddStress(int value)
        {
            Stress = UnityEngine.Mathf.Clamp(Stress + value, 0, 100);

            if (Stress >= 100)
            {
                IsActive = false;
            }
        }
    }
}
