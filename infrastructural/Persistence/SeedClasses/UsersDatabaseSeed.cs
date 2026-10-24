
using Microsoft.AspNetCore.Http;

namespace Persistence_Layer.Persistence.SeedClasses;

public static class UsersDatabaseSeed
{
    public static async Task SeedDatabaseAsync(ApplicationDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        var dummyUser = new User();
        var passwordHasher = new PasswordHasher<User>();
        string hashedRepoPassword = passwordHasher.HashPassword(dummyUser, "Password@1");
        
        var adminRole = await roleManager.FindByNameAsync("ADMIN");
        var userRole = await roleManager.FindByNameAsync("USER");
        var ownerRole = await roleManager.FindByNameAsync("OWNER");
        if (!context.Users.Any(u => u.RoleId == adminRole!.Id))
        {
            var user = new User() { UserName = "admin", Email = "admin@admin.com", EmailConfirmed = true, RoleId = adminRole!.Id };
            await userManager.CreateAsync(user, "Password@1");
        }
        if (!context.Users.Any(u => u.RoleId == userRole!.Id))
        {
            var baseurl = "https://localhost:5001/";
            var userFaker = new Faker<User>()
                .RuleFor(u => u.UserName, f => f.Internet.UserName())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.UserName))
                .RuleFor(u => u.Avatar, f => new Uri(new Uri(baseurl),"images/global/default.jpg").ToString())
                .RuleFor(u => u.CreatedAt, f => f.Date.Past().ToUniversalTime())
                // Apply the pre-generated hash
                .RuleFor(u => u.PasswordHash, _ => hashedRepoPassword)
                // Identity also needs these normalized for login to work!
                .RuleFor(u => u.NormalizedUserName, (f, u) => u.UserName?.ToUpper())
                .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email?.ToUpper())
                .RuleFor(u => u.SecurityStamp, _ => Guid.NewGuid().ToString())
                .RuleFor(u => u.RoleId, userRole!.Id)
                .RuleFor(u => u.EmailConfirmed, true);
        
            var fakeUsers = userFaker.Generate(100);
            await context.Users.AddRangeAsync(fakeUsers);
            await context.SaveChangesAsync();
        }
        
        if (!context.Users.Any(u => u.RoleId == ownerRole!.Id))
        {
            var baseurl = "https://localhost:5001/";
            var userFaker = new Faker<User>()
                .RuleFor(u => u.UserName, f => f.Internet.UserName())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.UserName)) // Use other properties
                .RuleFor(u => u.Avatar, f => new Uri(new Uri(baseurl),"images/global/default.jpg").ToString())
                .RuleFor(u => u.CreatedAt, f => f.Date.Past().ToUniversalTime())
                .RuleFor(u => u.RoleId, ownerRole!.Id)
                .RuleFor(u => u.PasswordHash, _ => hashedRepoPassword)
                .RuleFor(u => u.EmailConfirmed, true);
            
            var fakeUsers = userFaker.Generate(50);
            await context.AddRangeAsync(fakeUsers);
        }
        await context.SaveChangesAsync();
    }
}
