namespace CheckList.Web.Data.Entities;

public class CheckListEntity
{
    public int ListId { get; set; }
    public int SetId { get; set; }
    public string ListName { get; set; } = string.Empty;
    public string? ListDscr { get; set; }
    public string ActiveInd { get; set; } = "Y";
    public int SortOrder { get; set; } = 50;
    public DateTime CreateDateTime { get; set; }
    public string CreateUserName { get; set; } = "UNKNOWN";
    public DateTime ChangeDateTime { get; set; }
    public string ChangeUserName { get; set; } = "UNKNOWN";

    // Navigation
    public CheckSet Set { get; set; } = null!;
    public ICollection<CheckCategory> CheckCategories { get; set; } = [];
}
