namespace CheckList.Web.Data.Entities;

public class CheckAction
{
    public int ActionId { get; set; }
    public int CategoryId { get; set; }
    public int ListId { get; set; }
    public string ActionText { get; set; } = string.Empty;
    public string? ActionDscr { get; set; }
    public string CompleteInd { get; set; } = "N";
    public int SortOrder { get; set; } = 50;
    public DateTime CreateDateTime { get; set; }
    public string CreateUserName { get; set; } = "UNKNOWN";
    public DateTime ChangeDateTime { get; set; }
    public string ChangeUserName { get; set; } = "UNKNOWN";

    // Navigation
    public CheckCategory Category { get; set; } = null!;
}
