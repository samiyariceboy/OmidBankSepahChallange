using RedLockNet;
using RedLockNet.SERedis;
using StackExchange.Redis;
using RedLockNet.SERedis.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OmidBankSepah.MasterData.Infrastructure.Services.Cache;
using OmidBankSepah.MasterData.Application.Interfaces;
using OmidBankSepah.MasterData.Application.Services.Inventory;
using OmidBankSepah.MasterData.Domain.Services.DomainServices;

namespace OmidBankSepah.MasterData.Presentation.Registeration;
public static class ProductAvailabilityServiceRegistration
{
    public static IServiceCollection AddMasterDataServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration["Redis:ConnectionString"]
            ?? throw new InvalidOperationException("Redis:ConnectionString configuration is required.");

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = connectionString;
            options.InstanceName = "OmidBankSepah.MasterData:";
        });

        services.AddSingleton(_ => ConnectionMultiplexer.Connect(connectionString));
        services.AddSingleton<IConnectionMultiplexer>(
            provider => provider.GetRequiredService<ConnectionMultiplexer>());

        services.AddSingleton<IDistributedLockFactory>(provider =>
            RedLockFactory.Create(
                new List<RedLockMultiplexer>
                {
                    provider.GetRequiredService<ConnectionMultiplexer>()
                }));

        services.AddScoped<IProductAvailabilityCache, RedisCacheService>();
        services.AddScoped<IProductAvailabilityApplicationService, ProductAvailabilityApplicationService>();
        services.AddScoped<IProductAvailabilityDomainService, ProductAvailabilityDomainService>();

        return services;
    }
}
