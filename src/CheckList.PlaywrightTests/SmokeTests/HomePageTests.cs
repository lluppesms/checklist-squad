namespace CheckList.PlaywrightTests.SmokeTests;

[TestClass]
[TestCategory("Smoke")]
public class HomePageTests : SmokeTestBase
{
    [TestMethod]
    public async Task HomePage_LoadsSuccessfully_ShowsHeroTitle()
    {
        await NavigateRaw("/");

        var title = Page.Locator("h1.hero-title");
        await Expect(title).ToBeVisibleAsync();
        await Expect(title).ToContainTextAsync("Shared Checklist");
    }

    [TestMethod]
    public async Task HomePage_BeforeNickname_ShowsNicknameOverlay()
    {
        await NavigateRaw("/");

        var overlay = Page.Locator(".nickname-overlay");
        await Expect(overlay).ToBeVisibleAsync();

        var input = overlay.Locator("input[aria-label='Enter your nickname']");
        await Expect(input).ToBeVisibleAsync();

        var goButton = overlay.Locator("button.btn-primary");
        await Expect(goButton).ToBeDisabledAsync();
    }

    [TestMethod]
    public async Task HomePage_EnterNickname_OverlayDismissesSuccessfully()
    {
        await NavigateRaw("/");

        var overlay = Page.Locator(".nickname-overlay");
        await Expect(overlay).ToBeVisibleAsync();

        var input = overlay.Locator("input[aria-label='Enter your nickname']");
        var goButton = overlay.Locator("button.btn-primary");

        // Retry typing until Blazor processes the input (circuit may be connecting)
        for (var attempt = 0; attempt < 5; attempt++)
        {
            await input.ClickAsync();
            await input.FillAsync("");
            await input.PressSequentiallyAsync("TestUser", new LocatorPressSequentiallyOptions { Delay = 30 });
            await Page.WaitForTimeoutAsync(1000);

            try
            {
                await Expect(goButton).ToBeEnabledAsync(new LocatorAssertionsToBeEnabledOptions { Timeout = 3_000 });
                break;
            }
            catch (PlaywrightException) when (attempt < 4)
            {
                await Page.WaitForTimeoutAsync(2000);
            }
        }

        await goButton.ClickAsync();
        await Expect(overlay).ToBeHiddenAsync();

        // After nickname entry, the welcome text should update
        var welcome = Page.Locator(".welcome-text");
        await Expect(welcome).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task HomePage_AfterNickname_ShowsWelcomeContent()
    {
        await NavigateAndWaitForBlazor("/");

        // After dismissing nickname, the home page should show welcome content
        // Either quick links (if nickname persisted) or the prompt text
        var heroTitle = Page.Locator("h1.hero-title");
        await Expect(heroTitle).ToBeVisibleAsync();

        var welcomeContent = Page.Locator(".welcome-card");
        await Expect(welcomeContent).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task HomePage_PageTitle_IsCorrect()
    {
        await NavigateAndWaitForBlazor("/");
        await Expect(Page).ToHaveTitleAsync("Shared Checklist");
    }
}
