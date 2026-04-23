namespace CheckList.Web.Data.Repositories;

public class SharingRepository(CheckListDbContext db) : ISharingRepository
{
    public async Task<SharingInvite> CreateInviteAsync(SharingInvite invite)
    {
        db.SharingInvites.Add(invite);
        await db.SaveChangesAsync();
        return invite;
    }

    public async Task<SharingInvite?> GetInviteByTokenHashAsync(string tokenHash)
    {
        return await db.SharingInvites
            .Include(i => i.Sender)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.InviteTokenHash == tokenHash);
    }

    public async Task<List<SharingInvite>> GetInvitesForUserAsync(string userId)
    {
        return await db.SharingInvites
            .Where(i => i.SenderUserId == userId)
            .OrderByDescending(i => i.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AcceptInviteAsync(int inviteId, string acceptedByUserId)
    {
        var invite = await db.SharingInvites.FindAsync(inviteId)
            ?? throw new KeyNotFoundException($"Invite {inviteId} not found.");

        invite.Status = "accepted";
        invite.AcceptedByUserId = acceptedByUserId;
        invite.AcceptedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
    }

    public async Task<UserPartnership> CreatePartnershipAsync(UserPartnership partnership)
    {
        db.UserPartnerships.Add(partnership);
        await db.SaveChangesAsync();
        return partnership;
    }

    public async Task<List<UserPartnership>> GetPartnershipsForUserAsync(string userId)
    {
        return await db.UserPartnerships
            .Include(p => p.Partner)
            .Where(p => p.UserId == userId)
            .OrderBy(p => p.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task RevokePartnershipAsync(string userId, string partnerUserId)
    {
        var partnerships = await db.UserPartnerships
            .Where(p => (p.UserId == userId && p.PartnerUserId == partnerUserId) ||
                        (p.UserId == partnerUserId && p.PartnerUserId == userId))
            .ToListAsync();

        db.UserPartnerships.RemoveRange(partnerships);
        await db.SaveChangesAsync();
    }

    public async Task<List<string>> GetPartnerUserIdsAsync(string userId)
    {
        return await db.UserPartnerships
            .Where(p => p.UserId == userId && p.AutoShareEnabled)
            .Select(p => p.PartnerUserId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<CheckSetShare> CreateCheckSetShareAsync(CheckSetShare share)
    {
        db.CheckSetShares.Add(share);
        await db.SaveChangesAsync();
        return share;
    }

    public async Task<List<CheckSetShare>> GetSharesForCheckSetAsync(int checkSetId)
    {
        return await db.CheckSetShares
            .Where(s => s.CheckSetId == checkSetId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task DeleteSharesByPartnershipAsync(int partnershipId)
    {
        var shares = await db.CheckSetShares
            .Where(s => s.PartnershipId == partnershipId)
            .ToListAsync();

        db.CheckSetShares.RemoveRange(shares);
        await db.SaveChangesAsync();
    }
}
