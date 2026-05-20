using System;
using System.Collections.Generic;
using FOIA.Flow.Definitions;
using UnityEngine;

namespace FOIA.Flow.Runtime
{
    public sealed class AgencyRuntimeStore : MonoBehaviour
    {
        [SerializeField] private FoiaFlowDatabase database;

        private readonly List<AgencyRuntime> agencies = new();

        public event Action AgenciesChanged;
        public IReadOnlyList<AgencyRuntime> Agencies => agencies;

        private void Awake()
        {
            LoadFromDatabase();
        }

        public void LoadFromDatabase()
        {
            agencies.Clear();

            if (database != null)
            {
                foreach (AgencyDefinition definition in database.Agencies)
                {
                    if (definition != null)
                    {
                        agencies.Add(new AgencyRuntime(definition));
                    }
                }
            }

            AgenciesChanged?.Invoke();
        }

        public bool TryGetAgency(string agencyId, out AgencyRuntime runtime)
        {
            foreach (AgencyRuntime current in agencies)
            {
                if (current.Definition != null && current.Definition.AgencyId == agencyId)
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
            AgenciesChanged?.Invoke();
        }
    }
}
