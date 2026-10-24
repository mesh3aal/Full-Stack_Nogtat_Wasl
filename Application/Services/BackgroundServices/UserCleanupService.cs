
namespace Application_Layer.Services.BackgroundServices;

public class UserCleanupService(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                var expiryDate = DateTime.UtcNow.AddDays(-30);
                var usersToDelete = dbContext.Users
                    .Where(u => u.DeletedAt != null && u.DeletedAt <= expiryDate);

                dbContext.Users.RemoveRange(usersToDelete);
                await dbContext.SaveChangesAsync(stoppingToken);
            }
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}
