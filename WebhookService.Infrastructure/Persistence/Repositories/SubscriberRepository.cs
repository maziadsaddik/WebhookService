using Microsoft.EntityFrameworkCore;
using WebhookService.Appliaction.Contract.IRepositories;
using WebhookService.Domain.Entities;

namespace WebhookService.Infrastructure.Persistence.Repositories
{
    public class SubscriberRepository(AppDbContext dbContext)
        : Repository<Subscriber, Guid>(dbContext), ISubscriberRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<IReadOnlyList<Subscriber>> GetSubscribersByTenantIdAsync(
            string tenantId,
            CancellationToken cancellationToken
        ) => await _dbContext.Subscribers
                .AsNoTracking()
                .Where(s => s.TenantId == tenantId && s.IsActive)
                .ToListAsync(cancellationToken);
    }
}
