namespace CheckList.Api.Models.Entities;

public class TemplateCategory
{
    public int CategoryId { get; set; }
    public int ListId { get; set; }
    public string CategoryText { get; set; } = string.Empty;
    public string? CategoryDscr { get; set; }
    public string ActiveInd { get; set; } = "Y";
    public int SortOrder { get; set; } = 50;
    public DateTime CreateDateTime { get; set; }
    public string CreateUserName { get; set; } = "UNKNOWN";
    public DateTime ChangeDateTime { get; set; }
    public string ChangeUserName { get; set; } = "UNKNOWN";

    // Navigation
    public TemplateList List { get; set; } = null!;
    public ICollection<TemplateAction> TemplateActions { get; set; } = [];
}
