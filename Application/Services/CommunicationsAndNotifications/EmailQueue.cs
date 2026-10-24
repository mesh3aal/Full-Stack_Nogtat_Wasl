
namespace Application_Layer.Services.CommunicationsAndNotifications;

public class EmailQueue : IEmailQueue
{
    private readonly Channel<Func<IServiceProvider, Task>> _queue;

    public EmailQueue()
    {
        _queue = Channel.CreateUnbounded<Func<IServiceProvider, Task>>();
    }
    public void Enqueue(Func<IServiceProvider, Task> serviceItem)
    {
        _queue.Writer.TryWrite(serviceItem);
    }

    public async Task<Func<IServiceProvider, Task>> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}
