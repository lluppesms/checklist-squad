namespace CheckList.PlaywrightTests.SmokeTests;

[TestClass]
[TestCategory("Smoke")]
public class NavigationTests : SmokeTestBase
{
    [TestMethod]
    public async Task Navigation_SidebarLinksExist_AllFourPresent()
    {
        await NavigateAndWaitForBlazor("/");

        var sidebar = Page.Locator(".sidebar-nav");

        await Expect(sidebar.Locator("a[href='']")).ToBeVisibleAsync();
        await Expect(sidebar.Locator("a[href='templates']")).ToBeVisibleAsync();
        await Expect(sidebar.Locator("a[href='checklists']")).ToBeVisibleAsync();
        await Expect(sidebar.Locator("a[href='import-export']")).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Navigation_ClickTemplates_NavigatesToTemplatesPage()
    {
        await NavigateAndWaitForBlazor("/");

        await Page.Locator(".sidebar-nav a[href='templates']").ClickAsync();
        await Page.WaitForURLAsync($"**/templates");

        var pageTitle = Page.Locator("h2.page-title");
        await Expect(pageTitle).ToContainTextAsync("Blueprints");
    }

    [TestMethod]
    public async Task Navigation_ClickChecklists_NavigatesToChecklistsPage()
    {
        await NavigateAndWaitForBlazor("/");

        await Page.Locator(".sidebar-nav a[href='checklists']").ClickAsync();
        await Page.WaitForURLAsync($"**/checklists");

        var pageTitle = Page.Locator("h2.page-title");
        await Expect(pageTitle).ToContainTextAsync("My Checklists");
    }

    [TestMethod]
    public async Task Navigation_ClickImportExport_NavigatesToImportExportPage()
    {
        await NavigateAndWaitForBlazor("/");

        await Page.Locator(".sidebar-nav a[href='import-export']").ClickAsync();
        await Page.WaitForURLAsync($"**/import-export");

        var pageTitle = Page.Locator("h2.page-title");
        await Expect(pageTitle).ToContainTextAsync("Import/Export");
    }

    [TestMethod]
    public async Task Navigation_ClickHome_ReturnsToHomePage()
    {
        await NavigateAndWaitForBlazor("/templates");

        await Page.Locator(".sidebar-nav a[href='']").ClickAsync();

        // Wait for the home page hero to appear
        var heroTitle = Page.Locator("h1.hero-title");
        await Expect(heroTitle).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Navigation_BottomNavVisible_OnMobileViewport()
    {
        // Resize to mobile viewport
        await Page.SetViewportSizeAsync(375, 667);
        await NavigateAndWaitForBlazor("/");

        var bottomNav = Page.Locator(".bottom-nav");
        await Expect(bottomNav).ToBeVisibleAsync();
    }
}
