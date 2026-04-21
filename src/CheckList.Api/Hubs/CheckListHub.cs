namespace CheckList.Api.Hubs;

public class CheckListHub : Hub<ICheckListHubClient>
{
    public async Task JoinCheckSet(int checkSetId, string userName)
    {
        var groupName = $"checkset-{checkSetId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.OthersInGroup(groupName).UserJoined(userName, checkSetId);
    }

    public async Task LeaveCheckSet(int checkSetId, string userName)
    {
        var groupName = $"checkset-{checkSetId}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        await Clients.OthersInGroup(groupName).UserLeft(userName, checkSetId);
    }
}
