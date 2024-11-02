using MassTransit;
using MediatR;

namespace Anyding;

public abstract record Event : INotification
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTimeOffset Timestamp { get; set; } = DateTime.UtcNow;
}

public class MassTransitNotificationPublisher(IPublishEndpoint endpoint, IBus bus) : INotificationPublisher
{
    public async Task Publish(
        IEnumerable<NotificationHandlerExecutor> handlerExecutors,
        INotification notification,
        CancellationToken cancellationToken)
    {
        foreach (NotificationHandlerExecutor executor in handlerExecutors)
        {
            await executor.HandlerCallback(notification, cancellationToken);
        }

        await bus.Publish(notification, cancellationToken);
    }
}
