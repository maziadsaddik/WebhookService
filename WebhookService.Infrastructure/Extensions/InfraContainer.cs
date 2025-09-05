using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebhookService.Appliaction.Contract;
using WebhookService.Appliaction.Contract.IRepositories;
using WebhookService.Infrastructure.Persistence;
using WebhookService.Infrastructure.Persistence.Repositories;
using WebhookService.Infrastructure.Services;

namespace WebhookService.Infrastructure.Extensions
{
    public static class InfraContainer
    {
        public static IServiceCollection AddInfraServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("Database")));

            services.AddSingleton<ICryptoService>(provider =>
                new CryptoService(configuration["Security:MasterKey"]!));

            services.AddScoped<IWebhookDispatcher, WebhookDispatcher>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddHostedService<DatabaseMigrationHostedService>();

            return services;
        }
    }
}