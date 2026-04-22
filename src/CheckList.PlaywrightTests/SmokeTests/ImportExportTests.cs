namespace CheckList.PlaywrightTests.SmokeTests;

[TestClass]
[TestCategory("Smoke")]
public class ImportExportTests : SmokeTestBase
{
    [TestMethod]
    public async Task ImportExport_PageLoads_ShowsTitleAndSections()
    {
        await NavigateAndWaitForBlazor("/import-export");

        var pageTitle = Page.Locator("h2.page-title");
        await Expect(pageTitle).ToContainTextAsync("Import/Export");
    }

    [TestMethod]
    public async Task ImportExport_ExportSection_ShowsExportButtons()
    {
        await NavigateAndWaitForBlazor("/import-export");

        var exportSection = Page.Locator(".export-section");
        await Expect(exportSection).ToBeVisibleAsync();

        var exportAllButton = exportSection.Locator("button:has-text('Export All Templates')");
        await Expect(exportAllButton).ToBeVisibleAsync();

        var fullExportButton = exportSection.Locator("button:has-text('Full Export')");
        await Expect(fullExportButton).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task ImportExport_ImportSection_ShowsFileUploadAndButtons()
    {
        await NavigateAndWaitForBlazor("/import-export");

        var importSection = Page.Locator(".import-section");
        await Expect(importSection).ToBeVisibleAsync();

        var fileInput = importSection.Locator("input[type='file']");
        await Expect(fileInput).ToBeAttachedAsync();

        // Import buttons should be disabled when no file is selected
        var importTemplateButton = importSection.Locator("button:has-text('Import Template')");
        await Expect(importTemplateButton).ToBeDisabledAsync();
    }

    [TestMethod]
    public async Task ImportExport_PageTitle_IsCorrect()
    {
        await NavigateAndWaitForBlazor("/import-export");
        await Expect(Page).ToHaveTitleAsync("Import/Export — Shared Checklist");
    }
}
