using System.Text.RegularExpressions;

namespace CheckList.PlaywrightTests.SmokeTests;

/// <summary>
/// Base class for all Playwright smoke tests against the Shared Checklist app.
/// Each test gets its own browser context for isolation.
/// </summary>
[TestClass]
public class SmokeTestBase
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;

    protected IPage Page { get; private set; } = null!;

    protected static string BaseUrl =>
        Environment.GetEnvironmentVariable("APP_URL")
        ?? "https://lsq-checklist1-dev.azurewebsites.net";

    [TestInitialize]
    public async Task TestSetup()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        _context = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 720 },
            IgnoreHTTPSErrors = true
        });
        _context.SetDefaultTimeout(30_000);
        Page = await _context.NewPageAsync();
    }

    [TestCleanup]
    public async Task TestTeardown()
    {
        if (_context is not null) await _context.CloseAsync();
        if (_browser is not null) await _browser.CloseAsync();
        _playwright?.Dispose();
    }

    /// <summary>Playwright assertion helper.</summary>
    protected static ILocatorAssertions Expect(ILocator locator) => Assertions.Expect(locator);
    protected static IPageAssertions Expect(IPage page) => Assertions.Expect(page);

    /// <summary>
    /// Navigate to a route and wait for Blazor Server to become interactive.
    /// Dismisses the nickname overlay if present.
    /// </summary>
    protected async Task NavigateAndWaitForBlazor(string relativeUrl = "/")
    {
        var url = $"{BaseUrl.TrimEnd('/')}{relativeUrl}";
        await Page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.Load });
        await WaitForBlazorInteractive();
        await DismissNicknamePrompt();
    }

    /// <summary>
    /// Wait for Blazor Server to finish connecting the SignalR circuit
    /// so the page is actually interactive (not just prerendered HTML).
    /// </summary>
    private async Task WaitForBlazorInteractive()
    {
        // Wait for the app shell to appear (prerendered)
        await Page.WaitForSelectorAsync(".app-layout", new PageWaitForSelectorOptions { Timeout = 30_000 });

        // Wait for Blazor's SignalR circuit to connect.
        // The reconnect modal should not have the "open" attribute when connected.
        // We also check that the blazor error UI is hidden.
        await Page.WaitForFunctionAsync(@"() => {
            const modal = document.getElementById('components-reconnect-modal');
            const errorUI = document.getElementById('blazor-error-ui');
            return (!modal || !modal.hasAttribute('open')) && 
                   (!errorUI || errorUI.offsetParent === null);
        }", null, new PageWaitForFunctionOptions { Timeout = 30_000 });

        // Small extra wait for Blazor event handlers to attach
        await Page.WaitForTimeoutAsync(500);
    }

    /// <summary>
    /// If the nickname overlay is visible, enter a test nickname and proceed.
    /// </summary>
    protected async Task DismissNicknamePrompt()
    {
        var overlay = Page.Locator(".nickname-overlay");
        if (await overlay.IsVisibleAsync())
        {
            var input = overlay.Locator("input[aria-label='Enter your nickname']");
            var goButton = overlay.Locator("button.btn-primary");

            // Retry typing until Blazor processes the input (circuit may be connecting)
            for (var attempt = 0; attempt < 5; attempt++)
            {
                await input.ClickAsync();
                await input.FillAsync("");
                await input.PressSequentiallyAsync("Smoke-Tester", new LocatorPressSequentiallyOptions { Delay = 30 });
                await Page.WaitForTimeoutAsync(1000);

                try
                {
                    await Expect(goButton).ToBeEnabledAsync(new LocatorAssertionsToBeEnabledOptions { Timeout = 3_000 });
                    break;
                }
                catch (PlaywrightException) when (attempt < 4)
                {
                    // Blazor not ready yet — wait and retry
                    await Page.WaitForTimeoutAsync(2000);
                }
            }

            await goButton.ClickAsync();
            await overlay.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Hidden, Timeout = 10_000 });
        }
    }

    /// <summary>
    /// Navigate to a route without dismissing nickname (for testing the prompt itself).
    /// </summary>
    protected async Task NavigateRaw(string relativeUrl = "/")
    {
        var url = $"{BaseUrl.TrimEnd('/')}{relativeUrl}";
        await Page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.Load });
        await Page.WaitForSelectorAsync(".app-layout", new PageWaitForSelectorOptions { Timeout = 30_000 });
        // Wait for Blazor to become interactive so elements are wired up
        await Page.WaitForFunctionAsync(@"() => {
            const modal = document.getElementById('components-reconnect-modal');
            return !modal || !modal.hasAttribute('open');
        }", null, new PageWaitForFunctionOptions { Timeout = 30_000 });
        await Page.WaitForTimeoutAsync(500);
    }
}

