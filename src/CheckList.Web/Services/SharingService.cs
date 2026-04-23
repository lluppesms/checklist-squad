using System.Security.Cryptography;
using System.Text;

namespace CheckList.Web.Services;

public class SharingService(ISharingRepository sharingRepo, CheckListDbContext db) : ISharingService
{
    public async Task<(string inviteToken, SharingInvite invite)> CreateInviteAsync(string senderUserId, string recipientEmail, string role)
    {
        // Generate 32-byte random token and Base64Url encode it
        var tokenBytes = new byte[32];
        RandomNumberGenerator.Fill(tokenBytes);
        var inviteToken = Convert.ToBase64String(tokenBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .Replace("=", "");

        // SHA256 hash for storage
        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(inviteToken)));

        var invite = new SharingInvite
        {
            InviteTokenHash = tokenHash,
            SenderUserId = senderUserId,
            RecipientEmail = recipientEmail.ToLowerInvariant(),
            Role = role,
            Status = "pending",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        await sharingRepo.CreateInviteAsync(invite);

        return (inviteToken, invite);
    }

    public async Task<SharingInviteResult> AcceptInviteAsync(string inviteToken, string acceptorUserId, string acceptorEmail, string acceptorDisplayName)
    {
        // Hash the provided token and look up the invite
        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(inviteToken)));
        var invite = await sharingRepo.GetInviteByTokenHashAsync(tokenHash);

        if (invite == null)
        {
            return new SharingInviteResult(false, "Invalid or expired invite link.");
        }

        // Validate invite
        if (invite.Status != "pending")
        {
            return new SharingInviteResult(false, "This invite has already been used or revoked.");
        }

        if (invite.ExpiresAt < DateTime.UtcNow)
        {
            return new SharingInviteResult(false, "This invite has expired.");
        }

        if (!acceptorEmail.Equals(invite.RecipientEmail, StringComparison.OrdinalIgnoreCase))
        {
            return new SharingInviteResult(false, "This invite was sent to a different email address.");
        }

        // Ensure acceptor has an AppUser record
        var acceptor = await db.AppUsers.FindAsync(acceptorUserId);
        if (acceptor == null)
        {
            acceptor = new AppUser
            {
                UserId = acceptorUserId,
                DisplayName = acceptorDisplayName,
                Email = acceptorEmail,
                CreateDateTime = DateTime.UtcNow,
                LastLoginDateTime = DateTime.UtcNow
            };
            db.AppUsers.Add(acceptor);
            await db.SaveChangesAsync();
        }

        // Use a transaction for atomicity
        using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            // Create TWO UserPartnership rows (bidirectional)
            var partnership1 = new UserPartnership
            {
                UserId = invite.SenderUserId,
                PartnerUserId = acceptorUserId,
                Role = invite.Role,
                AutoShareEnabled = true,
                CreatedFromInviteId = invite.InviteId,
                CreatedAt = DateTime.UtcNow
            };

            var partnership2 = new UserPartnership
            {
                UserId = acceptorUserId,
                PartnerUserId = invite.SenderUserId,
                Role = invite.Role,
                AutoShareEnabled = true,
                CreatedFromInviteId = invite.InviteId,
                CreatedAt = DateTime.UtcNow
            };

            await sharingRepo.CreatePartnershipAsync(partnership1);
            await sharingRepo.CreatePartnershipAsync(partnership2);

            // Auto-share ALL existing checklists for both users
            await AutoShareExistingCheckSetsAsync(invite.SenderUserId, acceptorUserId, invite.Role, partnership1.PartnershipId);
            await AutoShareExistingCheckSetsAsync(acceptorUserId, invite.SenderUserId, invite.Role, partnership2.PartnershipId);

            // Mark invite as accepted
            await sharingRepo.AcceptInviteAsync(invite.InviteId, acceptorUserId);

            await transaction.CommitAsync();

            return new SharingInviteResult(true, PartnerDisplayName: invite.Sender.DisplayName);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<PartnerDto>> GetPartnersAsync(string userId)
    {
        var partnerships = await sharingRepo.GetPartnershipsForUserAsync(userId);

        return partnerships.Select(p => new PartnerDto(
            p.PartnerUserId,
            p.Partner.DisplayName,
            p.Partner.Email,
            p.Role,
            p.CreatedAt
        )).ToList();
    }

    public async Task RevokePartnershipAsync(string userId, string partnerUserId)
    {
        // Get the partnership IDs before deletion for cleanup
        var partnerships = await db.UserPartnerships
            .Where(p => (p.UserId == userId && p.PartnerUserId == partnerUserId) ||
                        (p.UserId == partnerUserId && p.PartnerUserId == userId))
            .ToListAsync();

        foreach (var partnership in partnerships)
        {
            await sharingRepo.DeleteSharesByPartnershipAsync(partnership.PartnershipId);
        }

        await sharingRepo.RevokePartnershipAsync(userId, partnerUserId);
    }

    public async Task AutoShareNewCheckSetAsync(int checkSetId, string ownerId)
    {
        var partnerUserIds = await sharingRepo.GetPartnerUserIdsAsync(ownerId);

        var checkSet = await db.CheckSets.FindAsync(checkSetId);
        if (checkSet == null) return;

        var partnerships = await db.UserPartnerships
            .Where(p => p.UserId == ownerId && partnerUserIds.Contains(p.PartnerUserId))
            .ToListAsync();

        foreach (var partnership in partnerships)
        {
            var share = new CheckSetShare
            {
                CheckSetId = checkSetId,
                SharedWithUserId = partnership.PartnerUserId,
                Role = partnership.Role,
                PartnershipId = partnership.PartnershipId,
                CreateDateTime = DateTime.UtcNow,
                CreateUserName = "System"
            };

            await sharingRepo.CreateCheckSetShareAsync(share);
        }
    }

    public async Task<bool> UserHasAccessAsync(string userId, int checkSetId, string? requiredRole = null)
    {
        var checkSet = await db.CheckSets
            .Include(s => s.CheckSetShares)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SetId == checkSetId);

        if (checkSet == null) return false;

        // Owner has full access
        if (checkSet.OwnerId == userId) return true;

        // Check if user has a share
        var share = checkSet.CheckSetShares.FirstOrDefault(s => s.SharedWithUserId == userId);
        if (share == null) return false;

        // If no specific role required, any share grants access
        if (requiredRole == null) return true;

        // Check if the share's role matches the required role
        return share.Role == requiredRole || share.Role == "admin";
    }

    private async Task AutoShareExistingCheckSetsAsync(string ownerId, string partnerUserId, string role, int partnershipId)
    {
        var checkSets = await db.CheckSets
            .Where(s => s.OwnerId == ownerId && s.ActiveInd == "Y")
            .ToListAsync();

        foreach (var checkSet in checkSets)
        {
            // Check if share already exists
            var existingShare = await db.CheckSetShares
                .FirstOrDefaultAsync(s => s.CheckSetId == checkSet.SetId && s.SharedWithUserId == partnerUserId);

            if (existingShare == null)
            {
                var share = new CheckSetShare
                {
                    CheckSetId = checkSet.SetId,
                    SharedWithUserId = partnerUserId,
                    Role = role,
                    PartnershipId = partnershipId,
                    CreateDateTime = DateTime.UtcNow,
                    CreateUserName = "System"
                };

                await sharingRepo.CreateCheckSetShareAsync(share);
            }
        }
    }
}
