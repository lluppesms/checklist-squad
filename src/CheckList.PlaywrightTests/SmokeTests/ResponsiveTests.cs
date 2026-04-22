namespace CheckList.PlaywrightTests.SmokeTests;

[TestClass]
[TestCategory("Smoke")]
public class ResponsiveTests : SmokeTestBase
{
    [TestMethod]
    public async Task Responsive_MobileViewport_HidesSidebarShowsBottomNav()
    {
        await Page.SetViewportSizeAsync(375, 667);
        await NavigateAndWaitForBlazor("/");

        // Sidebar should be hidden on mobile
        var sidebar = Page.Locator(".sidebar");
        await Expect(sidebar).ToBeHiddenAsync();

        // Bottom nav should be visible
        var bottomNav = Page.Locator(".bottom-nav");
        await Expect(bottomNav).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Responsive_DesktopViewport_ShowsSidebarHidesBottomNav()
    {
        await Page.SetViewportSizeAsync(1280, 720);
        await NavigateAndWaitForBlazor("/");

        var sidebar = Page.Locator(".sidebar");
        await Expect(sidebar).ToBeVisibleAsync();

        var bottomNav = Page.Locator(".bottom-nav");
        await Expect(bottomNav).ToBeHiddenAsync();
    }

    [TestMethod]
    public async Task Responsive_MobileViewport_HomePageIsUsable()
    {
        await Page.SetViewportSizeAsync(375, 667);
        await NavigateAndWaitForBlazor("/");

        var heroTitle = Page.Locator("h1.hero-title");
        await Expect(heroTitle).ToBeVisibleAsync();

        var welcomeCard = Page.Locator(".welcome-card");
        await Expect(welcomeCard).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Responsive_MobileViewport_BottomNavLinksWork()
    {
        await Page.SetViewportSizeAsync(375, 667);
        await NavigateAndWaitForBlazor("/");

        var bottomNav = Page.Locator(".bottom-nav");

        // Click Blueprints in bottom nav
        await bottomNav.Locator("a[href='templates']").ClickAsync();
        await Page.WaitForURLAsync("**/templates");

        var pageTitle = Page.Locator("h2.page-title");
        await Expect(pageTitle).ToContainTextAsync("Blueprints");
    }

    [TestMethod]
    public async Task Responsive_TabletViewport_AppRendersCorrectly()
    {
        await Page.SetViewportSizeAsync(768, 1024);
        await NavigateAndWaitForBlazor("/");

        // App shell should render
        var appLayout = Page.Locator(".app-layout");
        await Expect(appLayout).ToBeVisibleAsync();

        var heroTitle = Page.Locator("h1.hero-title");
        await Expect(heroTitle).ToBeVisibleAsync();
    }
}
