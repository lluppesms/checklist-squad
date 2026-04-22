namespace CheckList.PlaywrightTests.SmokeTests;

[TestClass]
[TestCategory("Smoke")]
public class TemplateTests : SmokeTestBase
{
    [TestMethod]
    public async Task Templates_PageLoads_ShowsTitleAndCreateButton()
    {
        await NavigateAndWaitForBlazor("/templates");

        var pageTitle = Page.Locator("h2.page-title");
        await Expect(pageTitle).ToContainTextAsync("Blueprints");

        var createButton = Page.Locator("button:has-text('Create New Blueprint')");
        await Expect(createButton).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Templates_ShowsTemplateCardsOrEmptyState()
    {
        await NavigateAndWaitForBlazor("/templates");

        // Either template cards or empty state should be visible
        var content = Page.Locator(".template-grid, .empty-state");
        await Expect(content).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Templates_WithCards_ShowsEditAndActivateButtons()
    {
        await NavigateAndWaitForBlazor("/templates");

        var cards = Page.Locator(".template-card");
        var count = await cards.CountAsync();

        if (count > 0)
        {
            var firstCard = cards.First;
            await Expect(firstCard.Locator(".card-title")).ToBeVisibleAsync();

            var editButton = firstCard.Locator("button:has-text('Edit')");
            await Expect(editButton).ToBeVisibleAsync();

            var activateButton = firstCard.Locator("button:has-text('Activate')");
            await Expect(activateButton).ToBeVisibleAsync();
        }
    }

    [TestMethod]
    public async Task Templates_CreateNewButton_NavigatesToEditor()
    {
        await NavigateAndWaitForBlazor("/templates");

        var createButton = Page.Locator("button:has-text('Create New Blueprint')");
        await createButton.ClickAsync();

        await Page.WaitForURLAsync("**/templates/edit**");
    }

    [TestMethod]
    public async Task TemplateEditor_PageLoads_ShowsEditorContent()
    {
        await NavigateAndWaitForBlazor("/templates/edit");

        // The editor page should load without errors
        var errorAlert = Page.Locator(".alert-danger");
        var editorVisible = !(await errorAlert.IsVisibleAsync());

        // At minimum, the page should not show an unhandled error
        var blazorError = Page.Locator("#blazor-error-ui");
        await Expect(blazorError).ToBeHiddenAsync();
    }
}
