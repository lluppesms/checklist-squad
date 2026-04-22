namespace CheckList.Web.Models;

public record ActivateCheckSetRequest(string OwnerName, List<int>? SelectedListIds = null, string? CustomName = null);

public record ToggleActionRequest(string UserName);
