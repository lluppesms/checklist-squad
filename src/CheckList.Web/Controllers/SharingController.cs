using System.Security.Claims;

namespace CheckList.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SharingController(
    ISharingService sharingService,
    IEmailService emailService,
    IUserIdentity userIdentity) : ControllerBase
{
    [HttpPost("invite")]
    public async Task<ActionResult<object>> CreateInvite([FromBody] CreateInviteRequest request)
    {
        var currentUserId = userIdentity.UserId;
        if (currentUserId == null)
        {
            return Unauthorized(new { message = "User not authenticated." });
        }

        try
        {
            var (inviteToken, invite) = await sharingService.CreateInviteAsync(
                currentUserId,
                request.RecipientEmail,
                request.Role ?? "user");

            var inviteLink = $"{Request.Scheme}://{Request.Host}/invite/{inviteToken}";

            await emailService.SendSharingInviteAsync(
                request.RecipientEmail,
                userIdentity.NickName ?? "A user",
                inviteLink,
                request.Role ?? "user");

            return Ok(new
            {
                inviteToken,
                inviteLink,
                expiresAt = invite.ExpiresAt
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("accept/{inviteToken}")]
    public async Task<ActionResult<object>> AcceptInvite(string inviteToken)
    {
        var currentUserId = userIdentity.UserId;
        var currentEmail = userIdentity.Email;
        var currentDisplayName = userIdentity.NickName;

        if (currentUserId == null || currentEmail == null || currentDisplayName == null)
        {
            return Unauthorized(new { message = "User not authenticated." });
        }

        try
        {
            var result = await sharingService.AcceptInviteAsync(
                inviteToken,
                currentUserId,
                currentEmail,
                currentDisplayName);

            if (!result.Success)
            {
                return BadRequest(new { success = false, error = result.ErrorMessage });
            }

            return Ok(new
            {
                success = true,
                partnerName = result.PartnerDisplayName
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    [HttpGet("partners")]
    public async Task<ActionResult<List<PartnerDto>>> GetPartners()
    {
        var currentUserId = userIdentity.UserId;
        if (currentUserId == null)
        {
            return Unauthorized(new { message = "User not authenticated." });
        }

        var partners = await sharingService.GetPartnersAsync(currentUserId);
        return Ok(partners);
    }

    [HttpDelete("partners/{partnerUserId}")]
    public async Task<IActionResult> RevokePartnership(string partnerUserId)
    {
        var currentUserId = userIdentity.UserId;
        if (currentUserId == null)
        {
            return Unauthorized(new { message = "User not authenticated." });
        }

        try
        {
            await sharingService.RevokePartnershipAsync(currentUserId, partnerUserId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public record CreateInviteRequest(string RecipientEmail, string? Role);
