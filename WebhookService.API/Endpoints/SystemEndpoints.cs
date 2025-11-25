using MediatR;
using Prometheus;
using System.Text;
using WebhookService.Appliaction.Dtos;

namespace WebhookService.API.Endpoints;

public static class SystemEndpoints
{
    public static void MapSystemEndpoints(this IEndpointRouteBuilder routes)
    {
        //Health check
        routes.MapGet("/health", async (IMediator mediator, CancellationToken cancellation) =>
        {
            string status = await mediator.Send(new HealthQuery(), cancellation);

            return status == "healthy" ? Results.Ok(new { status = status }) : Results.StatusCode(503);
        });

        // Metrics endpoint
        routes.MapGet("/metrics", () =>
        {
            using var stream = new MemoryStream();

            Metrics.DefaultRegistry.CollectAndExportAsTextAsync(stream);

            return Results.Text(Encoding.UTF8.GetString(stream.ToArray()), "text/plain");
        })
        .WithName("Prometheus metrics")
        .WithOpenApi();
    }
}