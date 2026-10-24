
namespace Application_Layer.Services.CommunicationsAndNotifications;

public interface IEmailService
{
    Task SendEmailAsync(EmailMessage emailMessage);
}
