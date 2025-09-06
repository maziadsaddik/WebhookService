using WebhookService.Domain.Entities;

namespace WebhookService.Appliaction.Contract.IRepositories;

public interface ISubscriberRepository : IRepository<Subscriber, Guid>
{
    Task<IReadOnlyList<Subscriber>> GetSubscribersByTenantIdAsync(string tenantId, CancellationToken cancellationToken);
}
