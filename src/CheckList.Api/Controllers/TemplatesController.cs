namespace CheckList.Api.Controllers;

using CheckList.Api.Hubs;
using CheckList.Api.Models.DTOs;
using CheckList.Api.Models.Mapping;

[ApiController]
[Route("api/[controller]")]
public class TemplatesController(ITemplateRepository templateRepo) : ControllerBase
{
    private const string DefaultUserName = "System";

    // Read operations
    [HttpGet]
    public async Task<ActionResult<List<TemplateSetSummaryDto>>> GetAll()
    {
        var sets = await templateRepo.GetAllSetsAsync();
        return Ok(sets.Select(s => s.ToSummaryDto()).ToList());
    }

    [HttpGet("{setId:int}")]
    public async Task<ActionResult<TemplateSetDto>> GetById(int setId)
    {
        var set = await templateRepo.GetSetWithHierarchyAsync(setId);
        if (set is null) return NotFound();
        return Ok(set.ToDto());
    }

    // Template Set CRUD
    [HttpPost]
    public async Task<ActionResult<TemplateSetDto>> CreateSet([FromBody] CreateTemplateSetRequest request)
    {
        var result = await templateRepo.CreateSetAsync(request, DefaultUserName);
        return CreatedAtAction(nameof(GetById), new { setId = result.SetId }, result);
    }

    [HttpPut("{setId:int}")]
    public async Task<ActionResult<TemplateSetDto>> UpdateSet(int setId, [FromBody] UpdateTemplateSetRequest request)
    {
        var result = await templateRepo.UpdateSetAsync(setId, request, DefaultUserName);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{setId:int}")]
    public async Task<ActionResult> DeleteSet(int setId)
    {
        var success = await templateRepo.DeleteSetAsync(setId);
        if (!success) return NotFound();
        return NoContent();
    }

    // Template List CRUD
    [HttpPost("{setId:int}/lists")]
    public async Task<ActionResult<TemplateListDto>> CreateList(int setId, [FromBody] CreateTemplateListRequest request)
    {
        var result = await templateRepo.CreateListAsync(setId, request, DefaultUserName);
        return CreatedAtAction(nameof(GetById), new { setId }, result);
    }

    [HttpPut("lists/{listId:int}")]
    public async Task<ActionResult<TemplateListDto>> UpdateList(int listId, [FromBody] UpdateTemplateListRequest request)
    {
        var result = await templateRepo.UpdateListAsync(listId, request, DefaultUserName);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("lists/{listId:int}")]
    public async Task<ActionResult> DeleteList(int listId)
    {
        var success = await templateRepo.DeleteListAsync(listId);
        if (!success) return NotFound();
        return NoContent();
    }

    // Template Category CRUD
    [HttpPost("lists/{listId:int}/categories")]
    public async Task<ActionResult<TemplateCategoryDto>> CreateCategory(int listId, [FromBody] CreateTemplateCategoryRequest request)
    {
        var result = await templateRepo.CreateCategoryAsync(listId, request, DefaultUserName);
        return Ok(result);
    }

    [HttpPut("categories/{categoryId:int}")]
    public async Task<ActionResult<TemplateCategoryDto>> UpdateCategory(int categoryId, [FromBody] UpdateTemplateCategoryRequest request)
    {
        var result = await templateRepo.UpdateCategoryAsync(categoryId, request, DefaultUserName);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("categories/{categoryId:int}")]
    public async Task<ActionResult> DeleteCategory(int categoryId)
    {
        var success = await templateRepo.DeleteCategoryAsync(categoryId);
        if (!success) return NotFound();
        return NoContent();
    }

    // Template Action CRUD
    [HttpPost("categories/{categoryId:int}/actions")]
    public async Task<ActionResult<TemplateActionDto>> CreateAction(int categoryId, [FromBody] CreateTemplateActionRequest request)
    {
        var result = await templateRepo.CreateActionAsync(categoryId, request, DefaultUserName);
        return Ok(result);
    }

    [HttpPut("actions/{actionId:int}")]
    public async Task<ActionResult<TemplateActionDto>> UpdateAction(int actionId, [FromBody] UpdateTemplateActionRequest request)
    {
        var result = await templateRepo.UpdateActionAsync(actionId, request, DefaultUserName);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("actions/{actionId:int}")]
    public async Task<ActionResult> DeleteAction(int actionId)
    {
        var success = await templateRepo.DeleteActionAsync(actionId);
        if (!success) return NotFound();
        return NoContent();
    }

    // Import/Export endpoints
    [HttpGet("{setId:int}/export")]
    public async Task<ActionResult<TemplateExportDto>> ExportSet(int setId)
    {
        try
        {
            var result = await templateRepo.ExportSetAsync(setId);
            return Ok(result);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    [HttpGet("export")]
    public async Task<ActionResult<List<TemplateExportDto>>> ExportAll()
    {
        var result = await templateRepo.ExportAllAsync();
        return Ok(result);
    }

    [HttpPost("import")]
    public async Task<ActionResult<TemplateSetDto>> ImportSet([FromBody] TemplateImportDto import)
    {
        var result = await templateRepo.ImportSetAsync(import, DefaultUserName);
        return CreatedAtAction(nameof(GetById), new { setId = result.SetId }, result);
    }
}

[ApiController]
[Route("api/export")]
public class ExportController(ITemplateRepository templateRepo) : ControllerBase
{
    [HttpGet("full")]
    public async Task<ActionResult<FullExportDto>> FullExport()
    {
        var result = await templateRepo.FullExportAsync();
        return Ok(result);
    }
}

[ApiController]
[Route("api/import")]
public class ImportController(ITemplateRepository templateRepo) : ControllerBase
{
    private const string DefaultUserName = "System";

    [HttpPost("full")]
    public async Task<ActionResult> FullImport([FromBody] FullImportDto import)
    {
        var (templateCount, checklistCount) = await templateRepo.FullImportAsync(import, DefaultUserName);
        return Ok(new { templateCount, checklistCount });
    }
}
