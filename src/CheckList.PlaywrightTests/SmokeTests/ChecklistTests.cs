namespace CheckList.PlaywrightTests.SmokeTests;

[TestClass]
[TestCategory("Smoke")]
public class ChecklistTests : SmokeTestBase
{
    [TestMethod]
    public async Task ActiveChecklists_PageLoads_ShowsTitleAndContent()
    {
        await NavigateAndWaitForBlazor("/checklists");

        var pageTitle = Page.Locator("h2.page-title");
        await Expect(pageTitle).ToContainTextAsync("My Checklists");

        // Should show either checklist cards or the empty state
        var content = Page.Locator(".checklist-grid, .empty-state");
        await Expect(content).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task ActiveChecklists_EmptyState_ShowsLinkToTemplates()
    {
        await NavigateAndWaitForBlazor("/checklists");

        var emptyState = Page.Locator(".empty-state");
        if (await emptyState.IsVisibleAsync())
        {
            var templateLink = emptyState.Locator("a[href='templates']");
            await Expect(templateLink).ToBeVisibleAsync();
        }
        // If there are checklists, that's also fine — smoke test passes
    }

    [TestMethod]
    public async Task ActiveChecklists_WithCards_ShowsChecklistInfo()
    {
        await NavigateAndWaitForBlazor("/checklists");

        var cards = Page.Locator(".checklist-card");
        var count = await cards.CountAsync();

        if (count > 0)
        {
            var firstCard = cards.First;
            await Expect(firstCard.Locator(".card-title")).ToBeVisibleAsync();
            await Expect(firstCard.Locator(".card-link")).ToHaveAttributeAsync("href", new System.Text.RegularExpressions.Regex(@"checklists/\d+"));
        }
        // Empty is also acceptable for smoke tests
    }

    [TestMethod]
    public async Task ChecklistView_NavigateToChecklist_ShowsContentOrError()
    {
        await NavigateAndWaitForBlazor("/checklists");

        var cards = Page.Locator(".checklist-card");
        var count = await cards.CountAsync();

        if (count > 0)
        {
            // Click into the first checklist
            await cards.First.Locator(".card-link").ClickAsync();
            await Page.WaitForURLAsync("**/checklists/*");

            // Should show checklist header with title and connection status
            var checklistTitle = Page.Locator(".checklist-title");
            await Expect(checklistTitle).ToBeVisibleAsync();

            var connectionStatus = Page.Locator(".connection-badge");
            await Expect(connectionStatus).ToBeVisibleAsync();
        }
    }

    [TestMethod]
    public async Task ChecklistView_CategoriesExpanded_ShowsActions()
    {
        await NavigateAndWaitForBlazor("/checklists");

        var cards = Page.Locator(".checklist-card");
        if (await cards.CountAsync() > 0)
        {
            await cards.First.Locator(".card-link").ClickAsync();
            await Page.WaitForURLAsync("**/checklists/*");

            // Wait for checklist content to load
            await Page.WaitForSelectorAsync(".checklist-header", new PageWaitForSelectorOptions { Timeout = 30_000 });

            // Categories should be visible (expanded by default)
            var categories = Page.Locator(".category-section");
            if (await categories.CountAsync() > 0)
            {
                var firstCategory = categories.First;
                await Expect(firstCategory.Locator(".category-title")).ToBeVisibleAsync();
            }
        }
    }

    [TestMethod]
    public async Task ChecklistView_ProgressBar_IsVisible()
    {
        await NavigateAndWaitForBlazor("/checklists");

        var cards = Page.Locator(".checklist-card");
        if (await cards.CountAsync() > 0)
        {
            await cards.First.Locator(".card-link").ClickAsync();
            await Page.WaitForURLAsync("**/checklists/*");
            await Page.WaitForSelectorAsync(".checklist-header", new PageWaitForSelectorOptions { Timeout = 30_000 });

            var progressBar = Page.Locator(".checklist-header .progress-bar, .checklist-header [role='progressbar']").First;
            await Expect(progressBar).ToBeVisibleAsync();
        }
    }
}
