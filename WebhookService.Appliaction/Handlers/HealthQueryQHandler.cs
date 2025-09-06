using MediatR;
using StackExchange.Redis;
using WebhookService.Appliaction.Contract.IRepositories;
using WebhookService.Appliaction.Dtos;

namespace WebhookService.Appliaction.Handlers
{
    public class HealthQueryQHandler(IUnitOfWork unitOfWork, IConnectionMultiplexer redis) : IRequestHandler<HealthQuery, string>
    {
        public async Task<string> Handle(HealthQuery query, CancellationToken cancellationToken)
        {
            try
            {
                await unitOfWork.SubscriberRepository.CanConnectAsync();

                await redis.GetDatabase().PingAsync();

                return "healthy";
            }
            catch
            {
                return "unhealthy";
            }
        }
    }
}
