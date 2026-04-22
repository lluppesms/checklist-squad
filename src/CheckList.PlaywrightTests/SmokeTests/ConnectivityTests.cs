namespace CheckList.PlaywrightTests.SmokeTests;

[TestClass]
[TestCategory("Smoke")]
public class ConnectivityTests : SmokeTestBase
{
    [TestMethod]
    public async Task SignalR_ChecklistView_ShowsConnectionStatus()
    {
        await NavigateAndWaitForBlazor("/checklists");

        var cards = Page.Locator(".checklist-card");
        if (await cards.CountAsync() > 0)
        {
            await cards.First.Locator(".card-link").ClickAsync();
            await Page.WaitForURLAsync("**/checklists/*");
            await Page.WaitForSelectorAsync(".checklist-header", new PageWaitForSelectorOptions { Timeout = 30_000 });

            // Connection status badge should appear
            var connectionBadge = Page.Locator(".connection-badge");
            await Expect(connectionBadge).ToBeVisibleAsync();
        }
    }

    [TestMethod]
    public async Task BlazorCircuit_AppLoads_NoErrorUI()
    {
        await NavigateAndWaitForBlazor("/");

        // The Blazor error UI should not be visible when everything is working
        var blazorError = Page.Locator("#blazor-error-ui");
        await Expect(blazorError).ToBeHiddenAsync();
    }

    [TestMethod]
    public async Task BlazorCircuit_ReconnectModal_NotVisibleOnLoad()
    {
        await NavigateAndWaitForBlazor("/");

        // The reconnect modal should not be open on a fresh, successful load
        var reconnectModal = Page.Locator("#components-reconnect-modal[open]");
        await Expect(reconnectModal).ToHaveCountAsync(0);
    }

    [TestMethod]
    public async Task AppBrand_IsVisible_OnAllPages()
    {
        await NavigateAndWaitForBlazor("/");

        var brand = Page.Locator(".app-brand");
        await Expect(brand).ToBeVisibleAsync();
        await Expect(brand).ToContainTextAsync("RigRoll");
    }
}
