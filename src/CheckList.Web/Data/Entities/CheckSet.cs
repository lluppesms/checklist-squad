namespace CheckList.Web.Data.Entities;

public class CheckSet
{
    public int SetId { get; set; }
    public int? TemplateSetId { get; set; }
    public string SetName { get; set; } = string.Empty;
    public string? SetDscr { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public string ActiveInd { get; set; } = "Y";
    public int SortOrder { get; set; } = 50;
    public DateTime CreateDateTime { get; set; }
    public string CreateUserName { get; set; } = "UNKNOWN";
    public DateTime ChangeDateTime { get; set; }
    public string ChangeUserName { get; set; } = "UNKNOWN";

    // Navigation
    public TemplateSet? TemplateSet { get; set; }
    public ICollection<CheckListEntity> CheckLists { get; set; } = [];
}
