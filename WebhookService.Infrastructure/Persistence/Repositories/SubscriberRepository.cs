using WebhookService.Appliaction.Contract.IRepositories;
using WebhookService.Domain.Entities;

namespace WebhookService.Infrastructure.Persistence.Repositories
{
    public class SubscriberRepository(AppDbContext dbContext)
        : Repository<Subscriber, Guid>(dbContext), ISubscriberRepository
    {
        private readonly AppDbContext _dbContext = dbContext;
    }
}
