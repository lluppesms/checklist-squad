namespace CheckList.Api.Models.DTOs;

public record ActivateCheckSetRequest(string OwnerName, List<int>? SelectedListIds = null);
