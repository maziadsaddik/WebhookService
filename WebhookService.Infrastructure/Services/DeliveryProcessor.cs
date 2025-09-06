using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebhookService.Appliaction.Contract;
using WebhookService.Domain.Entities;
using WebhookService.Domain.Enums;
using WebhookService.Infrastructure.Persistence;

namespace WebhookService.Infrastructure.Services
{
    public class DeliveryProcessor(
        IServiceProvider serviceProvider,
        ILogger<DeliveryProcessor> logger,
        IServiceScopeFactory scopeFactory
    ) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessPendingDeliveries(stoppingToken);

                    await Task.Delay(1000, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing deliveries");

                    await Task.Delay(5000, stoppingToken);
                }
            }
        }

        private async Task ProcessPendingDeliveries(CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var deliveries = await db.Deliveries
                .Include(d => d.Event)
                .Include(d => d.Subscriber)
                .Where(d => d.Status == nameof(Status.Pending) ||
                           (d.Status == nameof(Status.Failed) && d.NextRetryAt <= DateTime.UtcNow))
                .Take(10)
                .ToListAsync(cancellationToken);

            foreach (var delivery in deliveries)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                await ProcessDelivery(db, delivery);
            }

            await db.SaveChangesAsync(cancellationToken);
        }

        private async Task ProcessDelivery(AppDbContext db, Delivery delivery)
        {
            delivery.IncrementAttempt();

            if (delivery.Subscriber == null || delivery.Event == null)
            {
                logger.LogWarning(
                    "Delivery {DeliveryId} has null Subscriber or Event. Skipping.",
                    delivery.Id
                );
                delivery.MarkAsDlq("Subscriber or Event is null");
                return;
            }

            using (var scope = scopeFactory.CreateScope())
            {
                var dispatcher = scope.ServiceProvider.GetRequiredService<IWebhookDispatcher>();

                var result = await dispatcher.DispatchAsync(
                    delivery.Subscriber,
                    delivery.Event,
                    delivery
                );
                if (result.Success)
                {
                    delivery.MarkAsSuccess(result.HttpStatusCode ?? 0, result.DurationMs);

                    logger.LogInformation(
                        "Delivery succeeded: {DeliveryId} after {Attempts} attempts",
                        delivery.Id, delivery.AttemptNumber
                    );
                }
                else
                {
                    if (delivery.AttemptNumber >= 5)
                    {
                        delivery.MarkAsDlq(result.ErrorMessage);

                        logger.LogWarning(
                            "Delivery moved to DLQ: {DeliveryId} after {Attempts} attempts",
                            delivery.Id, delivery.AttemptNumber
                        );
                    }
                    else
                    {
                        delivery.MarkAsFailed(result.ErrorMessage, CalculateNextRetry(delivery.AttemptNumber));

                        logger.LogInformation(
                            "Delivery failed: {DeliveryId}, retry at {NextRetry}",
                            delivery.Id, delivery.NextRetryAt);
                    }
                }
            }
        }

        private DateTime CalculateNextRetry(int attemptNumber)
        {
            var baseDelays = new[] { 2, 10, 30, 120, 600 };

            var delay = baseDelays[Math.Min(attemptNumber, baseDelays.Length - 1)];

            // Add jitter (±20%)
            var jitter = Random.Shared.Next(-20, 20) * delay / 100;

            return DateTime.UtcNow.AddSeconds(delay + jitter);
        }
    }
}
