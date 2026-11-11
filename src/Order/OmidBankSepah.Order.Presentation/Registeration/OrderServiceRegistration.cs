using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OmidBankSepah.MasterData.Grpc;
using OmidBankSepah.Order.Application.Services.Orders;
using OmidBankSepah.Order.Domain.Repositories;
using OmidBankSepah.Order.Infrastructure.Repositories;
using OmidBankSepah.Order.Infrastructure.Services;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace OmidBankSepah.Order.Presentation.Registeration;

public static class OrderServiceRegistration
{
    public static IServiceCollection AddOrderServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var redisConnectionString = configuration["Redis:ConnectionString"]
            ?? throw new InvalidOperationException("Redis:ConnectionString configuration is required.");
        var masterDataGrpcAddress = configuration["Services:MasterDataGrpc"]
            ?? throw new InvalidOperationException("Services:MasterDataGrpc configuration is required.");

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddSingleton(_ => ConnectionMultiplexer.Connect(redisConnectionString));
        services.AddSingleton<IDistributedLockFactory>(provider =>
            RedLockFactory.Create(
                new List<RedLockMultiplexer>
                {
                    provider.GetRequiredService<ConnectionMultiplexer>()
                }));

        services.AddGrpcClient<ProductAvailabilityGrpc.ProductAvailabilityGrpcClient>(options =>
        {
            options.Address = new Uri(masterDataGrpcAddress);
        });
        services.AddScoped<IProductAvailabilityGrpcProxy, MasterDataProductAvailabilityGrpcProxy>();
        services.AddScoped<IOrderApplicationService, OrderApplicationService>();

        return services;
    }
}
