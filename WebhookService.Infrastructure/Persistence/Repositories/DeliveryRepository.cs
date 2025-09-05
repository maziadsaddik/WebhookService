using Microsoft.EntityFrameworkCore;
using WebhookService.Appliaction.Contract.IRepositories;
using WebhookService.Appliaction.Dtos;
using WebhookService.Domain.Entities;

namespace WebhookService.Infrastructure.Persistence.Repositories
{
    public class DeliveryRepository(AppDbContext dbContext)
        : Repository<Delivery, int>(dbContext), IDeliveryRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<(IReadOnlyList<Delivery>, int)> GetDeliveriesWithPaginationAsync(
            GetDeliveriesQuery query,
            CancellationToken cancellationToken
        )
        {
            IQueryable<Delivery> deliveries = _dbContext.Deliveries.AsNoTracking();

            if (query.EventId.HasValue)
                deliveries = deliveries.Where(d => d.EventId == query.EventId);

            if (query.SubscriberId.HasValue)
                deliveries = deliveries.Where(d => d.SubscriberId == query.SubscriberId);

            if (!string.IsNullOrEmpty(query.Status))
                deliveries = deliveries.Where(d => d.Status == query.Status);


            int totalRecords = await deliveries.CountAsync(cancellationToken);

            int lastPage = (int)Math.Ceiling(totalRecords / (double)query.PageSize);

            int skip = (query.CurrentPage - 1) * query.PageSize;

            return (await deliveries
                .Skip(skip)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken), lastPage);
        }
    }
}
