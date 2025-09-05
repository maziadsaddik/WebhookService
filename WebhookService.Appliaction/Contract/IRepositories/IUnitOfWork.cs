namespace WebhookService.Appliaction.Contract.IRepositories;

public interface IUnitOfWork : IDisposable
{
    ISubscriberRepository SubscriberRepository { get; }

    IDeliveryRepository DeliveryRepository { get; }

    IEventRepository EventRepository { get; }

    Task SaveChangeAsync(CancellationToken cancellationToken);
}
