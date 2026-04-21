namespace CheckList.Api.Controllers;

using CheckList.Api.Hubs;
using CheckList.Api.Models.DTOs;
using CheckList.Api.Models.Mapping;

[ApiController]
[Route("api/[controller]")]
public class TemplatesController(ITemplateRepository templateRepo) : ControllerBase
{
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
}
