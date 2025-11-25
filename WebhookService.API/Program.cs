using Prometheus;
using WebhookService.API.Endpoints;
using WebhookService.Appliaction.Extensions;
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

// Get the metrics service from DI
//Metrics
var eventCounter = Metrics.CreateCounter("swr_events_total", "Total events received");

var deliveryCounter = Metrics.CreateCounter("swr_deliveries_total", "Total deliveries", new[] { "status" });

var retryCounter = Metrics.CreateCounter("swr_retries_total", "Total retries");

var deliveryLatency = Metrics.CreateHistogram("swr_delivery_latency_ms", "Delivery latency in ms");

// Map endpoint groups
app.MapSubscriberEndpoints();
app.MapEventEndpoints(eventCounter);
app.MapSystemEndpoints();

app.Run();