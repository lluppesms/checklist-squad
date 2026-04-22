namespace CheckList.Web.Controllers;

using CheckList.Web.Data.Repositories;
using CheckList.Web.Hubs;
using CheckList.Web.Models;
using CheckList.Web.Models.Mapping;

[ApiController]
[Route("api/[controller]")]
public class CheckListsController(
    ICheckRepository checkRepo,
    IHubContext<CheckListHub, ICheckListHubClient> hubContext) : ControllerBase
{
    [HttpPost("activate/{templateSetId:int}")]
    public async Task<ActionResult<CheckSetDto>> Activate(int templateSetId, [FromBody] ActivateCheckSetRequest request)
    {
        try
        {
            var checkSet = await checkRepo.ActivateFromTemplateAsync(templateSetId, request.OwnerName, request.SelectedListIds, request.CustomName);
            await hubContext.Clients.All.CheckSetActivated(checkSet.SetId, checkSet.SetName);
            return CreatedAtAction(nameof(GetById), new { setId = checkSet.SetId }, checkSet.ToDto());
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<CheckSetSummaryDto>>> GetAll()
    {
        var sets = await checkRepo.GetAllActiveSetsAsync();
        return Ok(sets.Select(s => s.ToSummaryDto()).ToList());
    }

    [HttpGet("{setId:int}")]
    public async Task<ActionResult<CheckSetDto>> GetById(int setId)
    {
        var set = await checkRepo.GetSetWithHierarchyAsync(setId);
        if (set is null) return NotFound();
        return Ok(set.ToDto());
    }

    [HttpPut("actions/{actionId:int}/toggle")]
    public async Task<ActionResult<CheckActionDto>> ToggleAction(int actionId, [FromBody] ToggleActionRequest request)
    {
        try
        {
            var action = await checkRepo.ToggleActionAsync(actionId, request.UserName);
            var groupName = $"checkset-{action.ListId}";
            await hubContext.Clients.All.ActionToggled(action.ActionId, action.CompleteInd, request.UserName, action.ChangeDateTime);
            return Ok(action.ToDto());
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{setId:int}")]
    public async Task<IActionResult> Delete(int setId)
    {
        var deleted = await checkRepo.DeleteSetAsync(setId);
        if (!deleted) return NotFound();
        await hubContext.Clients.All.CheckSetDeleted(setId);
        return NoContent();
    }
}
