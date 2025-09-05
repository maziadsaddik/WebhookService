using MediatR;
using Microsoft.AspNetCore.Mvc;
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

// Get subscriber status
//app.MapGet("/api/subscribers/{id}/status", async (
//    Guid id,
//    ISubscriberService service) =>
//{
//    var status = await service.GetStatusAsync(id);
//    return Results.Ok(status);
//})
// .WithName("Subscriber status")
// .WithOpenApi();

// Ingest event
app.MapPost("/api/events", async (
    [FromBody] IngestEventInput input,
    [FromHeader(Name = "X-Idempotency-Key")] string idempotencyKey,
    IMediator mediator,
    CancellationToken cancellationToken) =>
{

    IngestEventCommand command = new()
    {
        TenantId = input.TenantId,
        EventType = input.EventType,
        Payload = input.Payload,
        IdempotencyKey = idempotencyKey
    };
    Event evt = await mediator.Send(command, cancellationToken);
    return Results.Created($"/api/events/{evt.Id}", new { id = evt.Id });
    // eventCounter.Inc();
    // var evt = await service.IngestAsync(request, idempotencyKey);
    //return Results.Created($"/api/events/{evt.Id}", new { id = evt.Id });
})
 .WithName("Ingest event")
 .WithOpenApi();


app.Run();

