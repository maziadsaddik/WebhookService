using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebhookService.API.Models.Inputs;
using WebhookService.Appliaction.Dtos;
using WebhookService.Domain.Entities;

namespace WebhookService.API.Endpoints;

public static class SubscriberEndpoints
{
    public static void MapSubscriberEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/subscribers");

        // Create subscriber
        group.MapPost("/", async (
            [FromBody] CreateSubscriberInput input,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            CreateSubscriberCommand command = new()
            {
                TenantId = input.TenantId,
                WebhookUrl = input.WebhookUrl,
                EventTypes = input.EventTypes,
            };

            Subscriber subscriber = await mediator.Send(command, cancellationToken);

            return Results.Created($"/api/subscribers/{subscriber.Id}", subscriber);
        })
        .WithName("Create subscriber")
        .WithOpenApi();

        // Rotate secret
        group.MapPost("/{id}/rotate-secret", async (
            Guid id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            RotateSecretCommand command = new() { Id = id };

            Subscriber subscriber = await mediator.Send(command, cancellationToken);

            return Results.Ok(new { message = "Secret rotated successfully" });
        })
        .WithName("Rotate secret")
        .WithOpenApi();

        // Get subscriber status
        group.MapGet("/{id}/status", async (
            Guid id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var status = await mediator.Send(new SubscriberStatusQuery { Id = id }, cancellationToken);
            return Results.Ok(status);
        })
        .WithName("Subscriber status")
        .WithOpenApi();
    }
}