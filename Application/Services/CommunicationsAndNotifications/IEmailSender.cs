
namespace Application_Layer.Services.CommunicationsAndNotifications;

public interface IEmailSender
{
    Task SendConfirmationEmailAsync(User user);
    Task SendConfirmationEmailAsync(User user, string userId, string email, string token, string baseUrl);
    Task SendForgotPasswordEmailAsync(User user, string baseUrl);
    Task DeactivateAccountAsync(User user, string baseUrl);
    Task ReactivateAccountAsync(User user, string urlProvider);
}
