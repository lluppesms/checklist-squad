namespace CheckList.Web.Services;

public interface IEmailService
{
    Task SendSharingInviteAsync(string recipientEmail, string senderName, string inviteLink, string role);
}
