namespace CheckList.Web.Data.Entities;

public class TemplateAction
{
    public int ActionId { get; set; }
    public int? CategoryId { get; set; }
    public string? ActionText { get; set; }
    public string? ActionDscr { get; set; }
    public string? CompleteInd { get; set; }
    public int SortOrder { get; set; } = 50;
    public DateTime CreateDateTime { get; set; }
    public string CreateUserName { get; set; } = "UNKNOWN";
    public DateTime ChangeDateTime { get; set; }
    public string ChangeUserName { get; set; } = "UNKNOWN";

    // Navigation
    public TemplateCategory? Category { get; set; }
}
