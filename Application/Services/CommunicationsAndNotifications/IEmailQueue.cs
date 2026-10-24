
namespace Application_Layer.Services.CommunicationsAndNotifications;

public interface IEmailQueue {
    void Enqueue(Func<IServiceProvider, Task> workItem);
    Task<Func<IServiceProvider, Task>> DequeueAsync(CancellationToken cancellationToken);
}
