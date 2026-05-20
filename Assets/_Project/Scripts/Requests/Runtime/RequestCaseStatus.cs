namespace FOIA.Requests.Runtime
{
    public enum RequestCaseStatus
    {
        Pending,
        Assigned,
        Working,
        WaitingResponse,
        Completed,
        ReadyToReview,
        Closed
    }
}
