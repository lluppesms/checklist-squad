namespace CheckList.Web.Data.Repositories;

public interface ISharingRepository
{
    Task<SharingInvite> CreateInviteAsync(SharingInvite invite);
    Task<SharingInvite?> GetInviteByTokenHashAsync(string tokenHash);
    Task<List<SharingInvite>> GetInvitesForUserAsync(string userId);
    Task AcceptInviteAsync(int inviteId, string acceptedByUserId);
    Task<UserPartnership> CreatePartnershipAsync(UserPartnership partnership);
    Task<List<UserPartnership>> GetPartnershipsForUserAsync(string userId);
    Task RevokePartnershipAsync(string userId, string partnerUserId);
    Task<List<string>> GetPartnerUserIdsAsync(string userId);
    Task<CheckSetShare> CreateCheckSetShareAsync(CheckSetShare share);
    Task<List<CheckSetShare>> GetSharesForCheckSetAsync(int checkSetId);
    Task DeleteSharesByPartnershipAsync(int partnershipId);
}
