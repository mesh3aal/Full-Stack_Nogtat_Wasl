using Microsoft.AspNetCore.Http;

namespace Persistence_Layer.Persistence;

public static class AutomatedMigration
{
    public static async Task MigrateAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        if ((context.Database.GetPendingMigrations()).Any()) context.Database.Migrate();

        var userManager = services.GetRequiredService<UserManager<User>>();
        
        var roleManager = services.GetRequiredService<RoleManager<Role>>();


        await DirectoratesAndDistricts.SeedDatabaseAsync(context);
        await DatabaseSeed.SeedDatabaseAsync(context, userManager, roleManager);
        await UsersDatabaseSeed.SeedDatabaseAsync(context, userManager, roleManager);
        await PlacesSeed.SeedDatabaseAsync(context,userManager);
        await PlacesSeed.SeedFakeData(context,userManager);
    }
}
