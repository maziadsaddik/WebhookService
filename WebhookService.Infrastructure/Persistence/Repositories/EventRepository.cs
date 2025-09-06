using Microsoft.EntityFrameworkCore;
using WebhookService.Appliaction.Contract.IRepositories;
using WebhookService.Domain.Entities;

namespace WebhookService.Infrastructure.Persistence.Repositories
{
    public class EventRepository(AppDbContext dbContext)
        : Repository<Event, Guid>(dbContext), IEventRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        public Task<bool> IsDuplicateAsync(string dedupKey, CancellationToken cancellationToken)
            => _dbContext.Events.AsNoTracking().AnyAsync(e => e.Payload == dedupKey, cancellationToken);
    }
}
