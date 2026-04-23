namespace CheckList.Web.Services;

public record SharingInviteResult(bool Success, string? ErrorMessage = null, string? PartnerDisplayName = null);
public record PartnerDto(string UserId, string DisplayName, string? Email, string Role, DateTime PartnerSince);

public interface ISharingService
{
    Task<(string inviteToken, SharingInvite invite)> CreateInviteAsync(string senderUserId, string recipientEmail, string role);
    Task<SharingInviteResult> AcceptInviteAsync(string inviteToken, string acceptorUserId, string acceptorEmail, string acceptorDisplayName);
    Task<List<PartnerDto>> GetPartnersAsync(string userId);
    Task RevokePartnershipAsync(string userId, string partnerUserId);
    Task AutoShareNewCheckSetAsync(int checkSetId, string ownerId);
    Task<bool> UserHasAccessAsync(string userId, int checkSetId, string? requiredRole = null);
}
