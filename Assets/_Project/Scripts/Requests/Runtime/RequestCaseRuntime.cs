using FOIA.Cards.Runtime;
using FOIA.Requests.Definitions;

namespace FOIA.Requests.Runtime
{
    public sealed class RequestCaseRuntime : IGameCardRuntime
    {
        public string CaseId { get; }
        public SO_RequestDefinition Definition { get; }

        public int ReceivedDay { get; }
        public int ReceivedHour { get; }
        public string AssignedOfficerId { get; private set; }
        public CardLocation Location { get; private set; }

        public RequestCaseStatus Status { get; private set; }
        public string RuntimeId => CaseId;
        public string Title => Definition.Title;

        public RequestCaseRuntime(
            string caseId,
            SO_RequestDefinition definition,
            int receivedDay,
            int receivedHour)
        {
            CaseId = caseId;
            Definition = definition;
            ReceivedDay = receivedDay;
            ReceivedHour = receivedHour;
            Status = RequestCaseStatus.Pending;
            Location = CardLocation.Inventory;
        }

        public void AssignOfficer(string officerId)
        {
            AssignedOfficerId = officerId;
            Status = RequestCaseStatus.Assigned;
        }

        public void ClearAssignedOfficer()
        {
            AssignedOfficerId = null;
            Status = RequestCaseStatus.Pending;
        }

        public void SetStatus(RequestCaseStatus status)
        {
            Status = status;
        }

        public void SetLocation(CardLocation location)
        {
            Location = location;
        }
    }
}
