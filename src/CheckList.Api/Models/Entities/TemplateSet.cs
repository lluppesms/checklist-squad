namespace CheckList.Api.Models.Entities;

public class TemplateSet
{
    public int SetId { get; set; }
    public string SetName { get; set; } = string.Empty;
    public string SetDscr { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string ActiveInd { get; set; } = "Y";
    public int SortOrder { get; set; } = 50;
    public DateTime CreateDateTime { get; set; }
    public string CreateUserName { get; set; } = "UNKNOWN";
    public DateTime ChangeDateTime { get; set; }
    public string ChangeUserName { get; set; } = "UNKNOWN";

    // Navigation
    public ICollection<TemplateList> TemplateLists { get; set; } = [];
    public ICollection<CheckSet> CheckSets { get; set; } = [];
}
