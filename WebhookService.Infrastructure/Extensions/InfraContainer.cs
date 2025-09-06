using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
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
            var configOptions = new ConfigurationOptions
            {
                EndPoints = { "localhost:6379" },
                AbortOnConnectFail = false
            };

            //var redisConnectionString = configuration.GetConnectionString("Redis");
            //if (string.IsNullOrEmpty(redisConnectionString))
            //    throw new InvalidOperationException("Redis connection string is not configured.");

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configOptions));

            //ConnectionMultiplexer.Connect(redisConnectionString));

            services.AddHttpClient();

            services.AddScoped<IWebhookDispatcher, WebhookDispatcher>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddHostedService<DeliveryProcessor>();

            services.AddHostedService<DatabaseMigrationHostedService>();

            return services;
        }
    }
}