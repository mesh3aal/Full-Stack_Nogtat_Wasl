
using Application_Layer.Services.CommunicationsAndNotifications;

namespace Application_Layer.Services.BackgroundServices;

public class EmailBackgroundWorker(IEmailQueue queue, IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested) {
            var workItem = await queue.DequeueAsync(stoppingToken);
            try {
                using var scope = serviceProvider.CreateScope();
                await workItem(scope.ServiceProvider);
            } catch (Exception ex) {
                // change it later to logger.
                Console.WriteLine(ex);
            }
        }
    }
}
