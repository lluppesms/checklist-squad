namespace CheckList.Tests.Models;

[TestClass]
public sealed class MappingTests
{
    // TemplateSet mappings
    [TestMethod]
    public void TemplateSet_ToSummaryDto_MapsAllFields()
    {
        var entity = CreateTemplateSet();
        var dto = entity.ToSummaryDto();

        Assert.AreEqual(1, dto.SetId);
        Assert.AreEqual("Camp Setup", dto.SetName);
        Assert.AreEqual("Desc", dto.SetDscr);
        Assert.AreEqual("Owner", dto.OwnerName);
        Assert.AreEqual("Y", dto.ActiveInd);
        Assert.AreEqual(10, dto.SortOrder);
    }

    [TestMethod]
    public void TemplateSet_ToDto_MapsWithLists()
    {
        var entity = CreateTemplateSetWithHierarchy();
        var dto = entity.ToDto();

        Assert.AreEqual(1, dto.SetId);
        Assert.AreEqual(1, dto.Lists.Count);
        Assert.AreEqual("Exterior", dto.Lists[0].ListName);
        Assert.AreEqual(1, dto.Lists[0].Categories.Count);
        Assert.AreEqual(1, dto.Lists[0].Categories[0].Actions.Count);
    }

    [TestMethod]
    public void TemplateSet_ToDto_EmptyLists()
    {
        var entity = CreateTemplateSet();
        var dto = entity.ToDto();

        Assert.AreEqual(0, dto.Lists.Count);
    }

    // TemplateList mapping
    [TestMethod]
    public void TemplateList_ToDto_MapsAllFields()
    {
        var entity = new TemplateList
        {
            ListId = 5, ListName = "Interior", ListDscr = "Inside", SortOrder = 2,
            TemplateCategories = []
        };
        var dto = entity.ToDto();

        Assert.AreEqual(5, dto.ListId);
        Assert.AreEqual("Interior", dto.ListName);
        Assert.AreEqual("Inside", dto.ListDscr);
        Assert.AreEqual(2, dto.SortOrder);
    }

    // TemplateCategory mapping
    [TestMethod]
    public void TemplateCategory_ToDto_MapsAllFields()
    {
        var entity = new TemplateCategory
        {
            CategoryId = 3, CategoryText = "Kitchen", CategoryDscr = "Cook stuff",
            SortOrder = 1, TemplateActions = []
        };
        var dto = entity.ToDto();

        Assert.AreEqual(3, dto.CategoryId);
        Assert.AreEqual("Kitchen", dto.CategoryText);
        Assert.AreEqual("Cook stuff", dto.CategoryDscr);
    }

    [TestMethod]
    public void TemplateCategory_ToDto_NullDescription()
    {
        var entity = new TemplateCategory
        {
            CategoryId = 1, CategoryText = "X", CategoryDscr = null,
            SortOrder = 1, TemplateActions = []
        };
        var dto = entity.ToDto();
        Assert.IsNull(dto.CategoryDscr);
    }

    // TemplateAction mapping
    [TestMethod]
    public void TemplateAction_ToDto_MapsAllFields()
    {
        var entity = new TemplateAction
        {
            ActionId = 7, ActionText = "Do this", ActionDscr = "Details", SortOrder = 3
        };
        var dto = entity.ToDto();

        Assert.AreEqual(7, dto.ActionId);
        Assert.AreEqual("Do this", dto.ActionText);
        Assert.AreEqual("Details", dto.ActionDscr);
        Assert.AreEqual(3, dto.SortOrder);
    }

    [TestMethod]
    public void TemplateAction_ToDto_NullFields()
    {
        var entity = new TemplateAction
        {
            ActionId = 1, ActionText = null, ActionDscr = null, SortOrder = 1
        };
        var dto = entity.ToDto();
        Assert.IsNull(dto.ActionText);
        Assert.IsNull(dto.ActionDscr);
    }

    // Export mappings
    [TestMethod]
    public void TemplateSet_ToExportDto_FiltersInactiveLists()
    {
        var entity = CreateTemplateSet();
        entity.TemplateLists =
        [
            new TemplateList
            {
                ListName = "Active", ListDscr = "D", ActiveInd = "Y", SortOrder = 1,
                TemplateCategories = []
            },
            new TemplateList
            {
                ListName = "Inactive", ListDscr = "D", ActiveInd = "N", SortOrder = 2,
                TemplateCategories = []
            }
        ];

        var dto = entity.ToExportDto();

        Assert.AreEqual(1, dto.Lists.Count);
        Assert.AreEqual("Active", dto.Lists[0].ListName);
    }

    [TestMethod]
    public void TemplateSet_ToExportDto_IncludesMetadata()
    {
        var entity = CreateTemplateSetWithHierarchy();
        var dto = entity.ToExportDto();

        Assert.IsNotNull(dto.Metadata);
        Assert.AreEqual("1.0.0", dto.Metadata.AppVersion);
        Assert.IsTrue(dto.Metadata.ItemCount > 0);
    }

    [TestMethod]
    public void TemplateCategory_ToExportDto_FiltersNothing()
    {
        var entity = new TemplateCategory
        {
            CategoryText = "Cat", CategoryDscr = "D", SortOrder = 1,
            TemplateActions =
            [
                new TemplateAction { ActionText = "A1", SortOrder = 1 },
                new TemplateAction { ActionText = "A2", SortOrder = 2 }
            ]
        };

        var dto = entity.ToExportDto();
        Assert.AreEqual(2, dto.Actions.Count);
    }

    // CheckSet mappings
    [TestMethod]
    public void CheckSet_ToSummaryDto_MapsAllFields()
    {
        var now = DateTime.UtcNow;
        var entity = new CheckSet
        {
            SetId = 1, TemplateSetId = 5, SetName = "Active", SetDscr = "D",
            OwnerName = "O", ActiveInd = "Y", SortOrder = 10, CreateDateTime = now
        };
        var dto = entity.ToSummaryDto();

        Assert.AreEqual(1, dto.SetId);
        Assert.AreEqual(5, dto.TemplateSetId);
        Assert.AreEqual("Active", dto.SetName);
        Assert.AreEqual(now, dto.CreateDateTime);
    }

    [TestMethod]
    public void CheckSet_ToDto_MapsWithLists()
    {
        var entity = CreateCheckSetWithHierarchy();
        var dto = entity.ToDto();

        Assert.AreEqual(1, dto.Lists.Count);
        Assert.AreEqual(1, dto.Lists[0].Categories.Count);
        Assert.AreEqual(1, dto.Lists[0].Categories[0].Actions.Count);
    }

    [TestMethod]
    public void CheckAction_ToDto_MapsAllFields()
    {
        var entity = new CheckAction
        {
            ActionId = 3, ActionText = "Do it", ActionDscr = "D",
            CompleteInd = "Y", ChangeUserName = "User", SortOrder = 1
        };
        var dto = entity.ToDto();

        Assert.AreEqual(3, dto.ActionId);
        Assert.AreEqual("Do it", dto.ActionText);
        Assert.AreEqual("Y", dto.CompleteInd);
        Assert.AreEqual("User", dto.ChangeUserName);
    }

    // CheckSet export mappings
    [TestMethod]
    public void CheckSet_ToExportDto_FiltersInactiveLists()
    {
        var entity = new CheckSet
        {
            SetName = "S", OwnerName = "O", SortOrder = 1,
            CheckLists =
            [
                new CheckListEntity
                {
                    ListName = "Active", ActiveInd = "Y", SortOrder = 1,
                    CheckCategories = []
                },
                new CheckListEntity
                {
                    ListName = "Inactive", ActiveInd = "N", SortOrder = 2,
                    CheckCategories = []
                }
            ]
        };

        var dto = entity.ToExportDto();
        Assert.AreEqual(1, dto.Lists.Count);
        Assert.AreEqual("Active", dto.Lists[0].ListName);
    }

    [TestMethod]
    public void CheckCategory_ToExportDto_IncludesAllActions()
    {
        var entity = new CheckCategory
        {
            CategoryText = "C", SortOrder = 1,
            CheckActions =
            [
                new CheckAction { ActionText = "A1", CompleteInd = "Y", SortOrder = 1 },
                new CheckAction { ActionText = "A2", CompleteInd = "N", SortOrder = 2 }
            ]
        };

        var dto = entity.ToExportDto();
        Assert.AreEqual(2, dto.Actions.Count);
        Assert.AreEqual("Y", dto.Actions[0].CompleteInd);
    }

    [TestMethod]
    public void CheckAction_ToExportDto_MapsFields()
    {
        var entity = new CheckAction
        {
            ActionText = "Test", ActionDscr = "D", CompleteInd = "N", SortOrder = 5
        };
        var dto = entity.ToExportDto();

        Assert.AreEqual("Test", dto.ActionText);
        Assert.AreEqual("N", dto.CompleteInd);
        Assert.AreEqual(5, dto.SortOrder);
    }

    // Helpers
    private static TemplateSet CreateTemplateSet() =>
        new()
        {
            SetId = 1, SetName = "Camp Setup", SetDscr = "Desc",
            OwnerName = "Owner", ActiveInd = "Y", SortOrder = 10
        };

    private static TemplateSet CreateTemplateSetWithHierarchy()
    {
        var set = CreateTemplateSet();
        set.TemplateLists =
        [
            new TemplateList
            {
                ListId = 1, ListName = "Exterior", ListDscr = "Outside", SortOrder = 1, ActiveInd = "Y",
                TemplateCategories =
                [
                    new TemplateCategory
                    {
                        CategoryId = 1, CategoryText = "Awnings", SortOrder = 1, ActiveInd = "Y",
                        TemplateActions =
                        [
                            new TemplateAction { ActionId = 1, ActionText = "Extend", SortOrder = 1 }
                        ]
                    }
                ]
            }
        ];
        return set;
    }

    private static CheckSet CreateCheckSetWithHierarchy() =>
        new()
        {
            SetId = 1, SetName = "Active", OwnerName = "O", ActiveInd = "Y", SortOrder = 1,
            CheckLists =
            [
                new CheckListEntity
                {
                    ListId = 1, ListName = "Exterior", ActiveInd = "Y", SortOrder = 1,
                    CheckCategories =
                    [
                        new CheckCategory
                        {
                            CategoryId = 1, CategoryText = "Awnings", ActiveInd = "Y", SortOrder = 1,
                            CheckActions =
                            [
                                new CheckAction
                                {
                                    ActionId = 1, ActionText = "Extend", CompleteInd = "N", SortOrder = 1
                                }
                            ]
                        }
                    ]
                }
            ]
        };
}
