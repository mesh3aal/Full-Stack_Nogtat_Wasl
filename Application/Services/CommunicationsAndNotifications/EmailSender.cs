
namespace Application_Layer.Services.CommunicationsAndNotifications;

public class EmailSender(UserManager<User> userManager,ITemplateService templateService, IEmailService emailService) : IEmailSender
{
    public async Task SendConfirmationEmailAsync(User user)
    {
        var pinCode = await userManager.GenerateUserTokenAsync(user, "Phone", "ConfirmEmailByPin");
        
        var emailBody = templateService.ReplaceInTemplate(await templateService.GetTemplateAsync(TemplateConstants.ConfirmEmail),new Dictionary<string, string> { { "{UserUserName}", user.UserName! }, { "{Token}",pinCode } });
        await emailService.SendEmailAsync(EmailMessage.Create(user.Email!,emailBody,"طلب تأكيد البريد الإلكتروني الخاصة بك"));
    }

    public async Task SendConfirmationEmailAsync(User user, string userId, string email, string token, string baseUrl)
    {
        var baseUri = new Uri(baseUrl);
        var param = new Dictionary<string, string?>
        {
            {"userId",userId },
            { "email", email },
            { "token", token }
        };
        var url = new Uri(baseUri, Routing.Authentication.EmailChangeConfirm).ToString();
        var encodedToken = QueryHelpers.AddQueryString(url, param);

        var emailBody = templateService.ReplaceInTemplate(await templateService.GetTemplateAsync(TemplateConstants.ConfirmEmail), new Dictionary<string, string> { { "{UserUserName}", user.UserName! }, { "{Token}", encodedToken } });
        await emailService.SendEmailAsync(EmailMessage.Create(email, emailBody, "طلب تأكيد البريد الإلكتروني الخاصة بك"));
    }
    public async Task SendForgotPasswordEmailAsync(User user,string baseUrl)
    {
        var totp6DigitPin = await userManager.GenerateUserTokenAsync(user,"Phone","ResetPasswordByPin");
        
        var emailBody = templateService.ReplaceInTemplate(await templateService.GetTemplateAsync(TemplateConstants.ForgetEmailTemp),new Dictionary<string, string> { { "{UserUserName}", user.UserName! }, { "{Token}",totp6DigitPin } });
        
        await emailService.SendEmailAsync(EmailMessage.Create(user.Email!,emailBody,"طلب إعادة تعيين كلمة المرور الخاصة بك"));
    }

    public async Task DeactivateAccountAsync(User user,string baseUrl)
    {
        var baseUri =  new Uri(baseUrl);
        var param = new Dictionary<string, string?>
        {
            { "email", user.Email! },
        };
        var url = new Uri(baseUri,Routing.CustomerSettings.Reactivate).ToString();
        var callback = QueryHelpers.AddQueryString(url, param);
        
        var emailBody = templateService.ReplaceInTemplate(await templateService.GetTemplateAsync(TemplateConstants.DeactivateAccount),new Dictionary<string, string> {{ "{ReactivateLink}",callback },{ "{UserUserName}", user.UserName! }});
        await emailService.SendEmailAsync(EmailMessage.Create(user.Email!,emailBody,"سنفتقد وجودك معنا.. تم تعطيل حسابك مؤقتاً طبيق نقطة وصل."));
    }

    public async Task ReactivateAccountAsync(User user,string urlProvider)
    {
        var url = $"{urlProvider}/Login";
        var emailBody = templateService.ReplaceInTemplate(await templateService.GetTemplateAsync(TemplateConstants.ReactivateAccount),new Dictionary<string, string> {{ "{UserUserName}", user.UserName! },{"{AppDashboardLink}",url}});
        await emailService.SendEmailAsync(EmailMessage.Create(user.Email!,emailBody,"سنفتقد وجودك معنا.. تم تعطيل حسابك مؤقتاً طبيق نقطة وصل."));
    }
}
