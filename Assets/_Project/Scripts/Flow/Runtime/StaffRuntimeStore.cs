using System;
using System.Collections.Generic;
using FOIA.Flow.Definitions;
using UnityEngine;

namespace FOIA.Flow.Runtime
{
    public sealed class StaffRuntimeStore : MonoBehaviour
    {
        [SerializeField] private FoiaFlowDatabase database;

        private readonly List<StaffRuntime> staff = new();

        public event Action StaffChanged;
        public IReadOnlyList<StaffRuntime> Staff => staff;

        private void Awake()
        {
            LoadFromDatabase();
        }

        public void LoadFromDatabase()
        {
            staff.Clear();

            if (database != null)
            {
                foreach (StaffDefinition definition in database.Staff)
                {
                    if (definition != null)
                    {
                        staff.Add(new StaffRuntime(definition));
                    }
                }
            }

            StaffChanged?.Invoke();
        }

        public bool TryGetStaff(string staffId, out StaffRuntime runtime)
        {
            foreach (StaffRuntime current in staff)
            {
                if (current.Definition != null && current.Definition.StaffId == staffId)
                {
                    runtime = current;
                    return true;
                }
            }

            runtime = null;
            return false;
        }

        public void NotifyChanged()
        {
            StaffChanged?.Invoke();
        }
    }
}
