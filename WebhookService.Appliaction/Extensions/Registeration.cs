using Microsoft.Extensions.DependencyInjection;
using WebhookService.Appliaction.Handlers;

namespace WebhookService.Appliaction.Extensions;

public static class Registeration
{
    public static void RegisterApplication(this IServiceCollection services)
    {
        services.AddMediatR(o => o.RegisterServicesFromAssemblyContaining<CreateSubscriberCommandHandler>());
    }
}
