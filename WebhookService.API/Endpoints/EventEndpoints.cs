using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prometheus;
using WebhookService.API.Models.Inputs;
using WebhookService.Appliaction.Dtos;

namespace WebhookService.API.Endpoints;

public static class EventEndpoints
{
    public static void MapEventEndpoints(this IEndpointRouteBuilder routes, Counter eventCounter)
    {
        // Events group
        var eventsGroup = routes.MapGroup("/api/events");

        // Ingest event
        eventsGroup.MapPost("/", async (
            [FromBody] IngestEventInput input,
            [FromHeader(Name = "X-Idempotency-Key")] string idempotencyKey,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            eventCounter.Inc();
            IngestEventCommand command = new()
            {
                TenantId = input.TenantId,
                EventType = input.EventType,
                Payload = input.Payload,
                IdempotencyKey = idempotencyKey
            };

            string eventId = await mediator.Send(command, cancellationToken);

            return Results.Created($"/api/events/{eventId}", new { id = eventId });
        })
        .WithName("Ingest event")
        .WithOpenApi();

        // Deliveries group
        var deliveriesGroup = routes.MapGroup("/api/deliveries");

        // Get deliveries
        deliveriesGroup.MapGet("/", async (
            [FromQuery] Guid? eventId,
            [FromQuery] Guid? subscriberId,
            [FromQuery] string? status,
            IMediator mediator,
            CancellationToken cancellationToken,
            [FromQuery] int currentPage = 1,
            [FromQuery] int pageSize = 20) =>
        {
            GetDeliveriesQuery query = new()
            {
                EventId = eventId,
                SubscriberId = subscriberId,
                Status = status,
                CurrentPage = currentPage,
                PageSize = pageSize
            };

            var (deliveries, totalPage) = await mediator.Send(query, cancellationToken);

            return Results.Ok(new { items = deliveries, totalPage = totalPage, pageSize = pageSize, currentPage });
        })
        .WithName("Delivery logs")
        .WithOpenApi();
    }
}