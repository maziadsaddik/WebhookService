using MediatR;
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


app.MapPost("/api/subscribers", async (CreateSubscriberInput input, IMediator mediator, CancellationToken cancellationToken) =>
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


app.Run();

