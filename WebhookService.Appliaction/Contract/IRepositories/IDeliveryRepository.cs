using WebhookService.Appliaction.Dtos;
using WebhookService.Domain.Entities;

namespace WebhookService.Appliaction.Contract.IRepositories;

public interface IDeliveryRepository : IRepository<Delivery, int>
{
    Task<(IReadOnlyList<Delivery>, int)> GetDeliveriesWithPaginationAsync(
                   GetDeliveriesQuery query,
                   CancellationToken cancellationToken
    );
}
