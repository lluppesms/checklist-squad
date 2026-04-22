namespace CheckList.Tests.Controllers;

[TestClass]
public sealed class ExportControllerTests
{
    [TestMethod]
    public async Task FullExport_ReturnsOk()
    {
        var repo = new Mock<ITemplateRepository>();
        var dto = new FullExportDto([], [], new ExportMetadata(DateTime.UtcNow, "1.0.0", 0));
        repo.Setup(r => r.FullExportAsync()).ReturnsAsync(dto);

        var controller = new ExportController(repo.Object);
        var result = await controller.FullExport();

        Assert.IsInstanceOfType<OkObjectResult>(result.Result);
    }
}

[TestClass]
public sealed class ImportControllerTests
{
    [TestMethod]
    public async Task FullImport_ReturnsOk_WithCounts()
    {
        var repo = new Mock<ITemplateRepository>();
        var import = new FullImportDto([], []);
        repo.Setup(r => r.FullImportAsync(import, "System")).ReturnsAsync((2, 3));

        var controller = new ImportController(repo.Object);
        var result = await controller.FullImport(import);

        Assert.IsInstanceOfType<OkObjectResult>(result);
    }
}
