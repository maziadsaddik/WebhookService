using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prometheus;
using System.Text;
using WebhookService.API.Models.Inputs;
using WebhookService.Appliaction.Dtos;
using WebhookService.Appliaction.Extensions;
using WebhookService.Domain.Entities;
using WebhookService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddInfraServices(builder.Configuration);

builder.Services.RegisterApplication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Metrics
var eventCounter = Metrics.CreateCounter("swr_events_total", "Total events received");

var deliveryCounter = Metrics.CreateCounter("swr_deliveries_total", "Total deliveries", new[] { "status" });

var retryCounter = Metrics.CreateCounter("swr_retries_total", "Total retries");

var deliveryLatency = Metrics.CreateHistogram("swr_delivery_latency_ms", "Delivery latency in ms");

//Api 
app.MapPost("/api/subscribers", async ([FromBody] CreateSubscriberInput input, IMediator mediator, CancellationToken cancellationToken) =>
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

app.MapPost("/api/subscribers/{id}/rotate-secret", async (
    Guid id,
    IMediator mediator,
    CancellationToken cancellationToken
) =>
{
    RotateSecretCommand command = new() { Id = id };

    Subscriber subscriber = await mediator.Send(command, cancellationToken);

    return Results.Ok(new { message = "Secret rotated successfully" });
})
 .WithName("Rotate secret")
 .WithOpenApi();

//Get subscriber status
app.MapGet("/api/subscribers/{id}/status", async (
   Guid id,
   IMediator mediator,
   CancellationToken cancellationToken
) =>
{
    var status = await mediator.Send(new SubscriberStatusQuery { Id = id }, cancellationToken);
    return Results.Ok(status);
})
 .WithName("Subscriber status")
 .WithOpenApi();

// Ingest event
app.MapPost("/api/events", async (
    [FromBody] IngestEventInput input,
    [FromHeader(Name = "X-Idempotency-Key")] string idempotencyKey,
    IMediator mediator,
    CancellationToken cancellationToken
) =>
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

// Get deliveries
app.MapGet("/api/deliveries", async (
    [FromQuery] Guid? eventId,
    [FromQuery] Guid? subscriberId,
    [FromQuery] string? status,
    IMediator mediator,
    CancellationToken cancellationToken,
    [FromQuery] int currentPage = 1,
    [FromQuery] int pageSize = 20
) =>
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

//Health check
app.MapGet("/health", async (IMediator mediator, CancellationToken cancellation) =>
{
    string status = await mediator.Send(new HealthQuery(), cancellation);

    return status == "healthy" ? Results.Ok(new { status = status }) : Results.StatusCode(503);
});


// Metrics endpoint
app.MapGet("/metrics", () =>
{
    using var stream = new MemoryStream();

    Metrics.DefaultRegistry.CollectAndExportAsTextAsync(stream);

    return Results.Text(Encoding.UTF8.GetString(stream.ToArray()), "text/plain");
})
 .WithName("Prometheus metrics")
 .WithOpenApi();

app.Run();

