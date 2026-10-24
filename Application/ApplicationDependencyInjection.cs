using Application_Layer.Services;
using Application_Layer.Services.AnalyticsAndDashboards;
using Application_Layer.Services.CatalogAndClassifications;
using Application_Layer.Services.CommunicationsAndNotifications;
using Application_Layer.Services.IdentityAndAccess;
using Application_Layer.Services.InfrastructureAndUtilities;
using Application_Layer.Services.InteractionsAndFeedback;
using Application_Layer.Services.Notifications;

namespace Application_Layer;

public static class ApplicationDependencyInjection
{
    public static void AddApplication(this IServiceCollection services,IConfiguration configuration,IWebHostEnvironment env)
    {
        services.AddServices();
        services.AddEmailConfiguration(configuration);
        services.RegisterWwwRoot(env);
    }

    private static void AddServices(this IServiceCollection services)
    {
        // Scoped Lifetime
        services.AddHttpContextAccessor();
        services.AddScoped<IAuthService,AuthService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ITemplateService, TemplateService>();
        services.AddScoped<IUserSettingsService, UserSettingsService>();
        services.AddScoped<IPlaceService, PlaceService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IFavouriteService, FavouriteService>();
        services.AddScoped<ISubCategoryService, SubCategoryService>();
        services.AddScoped<IUrlProvider, UrlProvider>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IStoreManagementService, StoreManagementService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IOwnerDashboardService, OwnerDashboardService>();
        services.AddScoped<IRequestService, RequestService>();
        services.AddScoped<IAdminDashboardService, AdminDashboardService>();
        services.AddScoped<INotificationService, NotificationService>();
        
        // singleton Services
        services.AddSingleton<IEmailQueue, EmailQueue>();
        
      // background services
        services.AddHostedService<EmailBackgroundWorker>();
        services.AddHostedService<UserCleanupService>();
    }
    private static void AddEmailConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration.GetSection("SmtpSettings").Get<SmtpSettings>());
    }

    private static void RegisterWwwRoot(this IServiceCollection services,IWebHostEnvironment env)
    {
        var wwwrootPath = Path.Combine(env.ContentRootPath, "wwwroot");
        if (!Directory.Exists(wwwrootPath))
        {
            Directory.CreateDirectory(wwwrootPath);
        }

        Directory.CreateDirectory(Path.Combine(env.WebRootPath, "images/global"));
    }
}
