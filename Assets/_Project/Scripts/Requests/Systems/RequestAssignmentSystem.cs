using System;
using FOIA.Officers.Runtime;
using FOIA.Requests.Runtime;

namespace FOIA.Requests.Systems
{
    public sealed class RequestAssignmentSystem
    {
        public void AssignOfficer(RequestCaseRuntime requestCase, OfficerRuntime officer)
        {
            if (requestCase == null)
            {
                throw new ArgumentNullException(nameof(requestCase));
            }

            if (officer == null)
            {
                throw new ArgumentNullException(nameof(officer));
            }

            if (officer.Status == OfficerStatus.Resigned)
            {
                return;
            }

            requestCase.AssignOfficer(officer.RuntimeId);
        }

        public void ClearOfficer(RequestCaseRuntime requestCase)
        {
            if (requestCase == null)
            {
                throw new ArgumentNullException(nameof(requestCase));
            }

            requestCase.ClearAssignedOfficer();
        }
    }
}
