namespace Persistence_Layer;

public static class InfrastrucureDependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDataBase(configuration);
        services.AddIdentity();
    }
    
    private static void AddDataBase(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseConfig =  configuration.GetSection("Database").Get<DatabaseModel>();
        if (databaseConfig == null)
        {
            throw new InvalidOperationException("Database configuration is invalid");
        }
        
        services.AddDbContext<ApplicationDbContext>(opt =>
        {
            opt.UseNpgsql(databaseConfig.ConnectionString, options =>
            {
                options.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            });
        });
    }
    private static void AddIdentity(this IServiceCollection services)
    {

        services.AddIdentityCore<User>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;

                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 1;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireNonAlphanumeric = false;

                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
    }
}

public class DatabaseModel
{
    public string? ConnectionString { get; set; }
}
