namespace CheckList.Api.Hubs;

public interface ICheckListHubClient
{
    Task ActionToggled(int actionId, string completeInd, string userName, DateTime changedAt);
    Task CheckSetActivated(int checkSetId, string setName);
    Task CheckSetDeleted(int checkSetId);
    Task UserJoined(string userName, int checkSetId);
    Task UserLeft(string userName, int checkSetId);
}
