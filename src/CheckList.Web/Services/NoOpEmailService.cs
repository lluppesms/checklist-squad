namespace CheckList.Web.Services;

/// <summary>
/// No-op email service that logs invite links instead of sending emails.
/// Replace with a real email service implementation when email sending is configured.
/// </summary>
public class NoOpEmailService(ILogger<NoOpEmailService> logger) : IEmailService
{
    public Task SendSharingInviteAsync(string recipientEmail, string senderName, string inviteLink, string role)
    {
        logger.LogInformation(
            "Sharing invite for {RecipientEmail} from {SenderName} with role {Role}. " +
            "Invite link: {InviteLink}",
            recipientEmail, senderName, role, inviteLink);

        return Task.CompletedTask;
    }
}
