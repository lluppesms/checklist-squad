using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CheckList.Tests.Helpers;

public static class DbContextHelper
{
    public static CheckListDbContext CreateInMemoryContext(string? dbName = null)
    {
        dbName ??= Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<CheckListDbContext>()
            .UseInMemoryDatabase(dbName)
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new CheckListDbContext(options);
    }

    public static TemplateSet SeedTemplateSet(CheckListDbContext db, string name = "Camp Setup", string owner = "TestUser")
    {
        var now = DateTime.UtcNow;
        var set = new TemplateSet
        {
            SetName = name,
            SetDscr = "Test template set",
            OwnerName = owner,
            ActiveInd = "Y",
            SortOrder = 10,
            CreateDateTime = now,
            CreateUserName = owner,
            ChangeDateTime = now,
            ChangeUserName = owner,
            TemplateLists =
            [
                new TemplateList
                {
                    ListName = "Exterior",
                    ListDscr = "Outside tasks",
                    ActiveInd = "Y",
                    SortOrder = 1,
                    CreateDateTime = now,
                    CreateUserName = owner,
                    ChangeDateTime = now,
                    ChangeUserName = owner,
                    TemplateCategories =
                    [
                        new TemplateCategory
                        {
                            CategoryText = "Awnings",
                            CategoryDscr = "Awning tasks",
                            ActiveInd = "Y",
                            SortOrder = 1,
                            CreateDateTime = now,
                            CreateUserName = owner,
                            ChangeDateTime = now,
                            ChangeUserName = owner,
                            TemplateActions =
                            [
                                new TemplateAction
                                {
                                    ActionText = "Extend main awning",
                                    ActionDscr = "Pull out the main awning",
                                    SortOrder = 1,
                                    CreateDateTime = now,
                                    CreateUserName = owner,
                                    ChangeDateTime = now,
                                    ChangeUserName = owner
                                },
                                new TemplateAction
                                {
                                    ActionText = "Secure awning legs",
                                    ActionDscr = null,
                                    SortOrder = 2,
                                    CreateDateTime = now,
                                    CreateUserName = owner,
                                    ChangeDateTime = now,
                                    ChangeUserName = owner
                                }
                            ]
                        }
                    ]
                },
                new TemplateList
                {
                    ListName = "Interior",
                    ListDscr = "Inside tasks",
                    ActiveInd = "Y",
                    SortOrder = 2,
                    CreateDateTime = now,
                    CreateUserName = owner,
                    ChangeDateTime = now,
                    ChangeUserName = owner,
                    TemplateCategories =
                    [
                        new TemplateCategory
                        {
                            CategoryText = "Kitchen",
                            CategoryDscr = null,
                            ActiveInd = "Y",
                            SortOrder = 1,
                            CreateDateTime = now,
                            CreateUserName = owner,
                            ChangeDateTime = now,
                            ChangeUserName = owner,
                            TemplateActions =
                            [
                                new TemplateAction
                                {
                                    ActionText = "Stock fridge",
                                    ActionDscr = null,
                                    SortOrder = 1,
                                    CreateDateTime = now,
                                    CreateUserName = owner,
                                    ChangeDateTime = now,
                                    ChangeUserName = owner
                                }
                            ]
                        }
                    ]
                }
            ]
        };

        db.TemplateSets.Add(set);
        db.SaveChanges();
        return set;
    }

    public static CheckSet SeedCheckSet(CheckListDbContext db, string name = "Active Checklist", string owner = "TestUser")
    {
        var now = DateTime.UtcNow;
        var set = new CheckSet
        {
            SetName = name,
            SetDscr = "Test check set",
            OwnerName = owner,
            ActiveInd = "Y",
            SortOrder = 10,
            CreateDateTime = now,
            CreateUserName = owner,
            ChangeDateTime = now,
            ChangeUserName = owner,
            CheckLists =
            [
                new CheckListEntity
                {
                    ListName = "Exterior",
                    ListDscr = "Outside tasks",
                    ActiveInd = "Y",
                    SortOrder = 1,
                    CreateDateTime = now,
                    CreateUserName = owner,
                    ChangeDateTime = now,
                    ChangeUserName = owner,
                    CheckCategories =
                    [
                        new CheckCategory
                        {
                            CategoryText = "Awnings",
                            CategoryDscr = "Awning tasks",
                            ActiveInd = "Y",
                            SortOrder = 1,
                            CreateDateTime = now,
                            CreateUserName = owner,
                            ChangeDateTime = now,
                            ChangeUserName = owner,
                            CheckActions =
                            [
                                new CheckAction
                                {
                                    ActionText = "Extend main awning",
                                    ActionDscr = "Pull it out",
                                    CompleteInd = "N",
                                    SortOrder = 1,
                                    CreateDateTime = now,
                                    CreateUserName = owner,
                                    ChangeDateTime = now,
                                    ChangeUserName = owner
                                }
                            ]
                        }
                    ]
                }
            ]
        };

        db.CheckSets.Add(set);
        db.SaveChanges();
        return set;
    }
}
