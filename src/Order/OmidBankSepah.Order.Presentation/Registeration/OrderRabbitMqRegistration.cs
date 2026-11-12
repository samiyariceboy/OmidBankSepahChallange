using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OmidBankSepah.Order.Infrastructure.Configuration;
using OmidBankSepah.Order.Infrastructure.Services;
using RabbitMQ.Client;
using Taday.Corelibrary.Common.Shared;
using Taday.Corelibrary.Infrastucture.Services.Broker;

namespace OmidBankSepah.Order.Presentation.Registeration;

public static class OrderRabbitMqRegistration
{
    public static IServiceCollection AddOrderRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = configuration.GetSection("RabbitMq").Get<OrderRabbitMqSettings>()
            ?? throw new InvalidOperationException("RabbitMq configuration is required.");
        settings.GetRabbitMqQueueConfig(settings.ProductInventoryQueue);

        services.AddSingleton<
            IAppLogger<RabbitMqBaseService>,
            Taday.Corelibrary.Common.Shared.Logger<RabbitMqBaseService>>();
        services.AddSingleton(settings);
        services.AddSingleton(CreateConnectionFactory(settings));
        services.AddSingleton<OrderMessageBrokerService>();
        services.AddHostedService(provider =>
            provider.GetRequiredService<OrderMessageBrokerService>());

        return services;
    }

    private static ConnectionFactory CreateConnectionFactory(OrderRabbitMqSettings settings)
    {
        var connectionFactory = new ConnectionFactory
        {
            HostName = settings.HostName,
            UserName = settings.UserName,
            Password = settings.Password,
            Port = settings.Port,
            DispatchConsumersAsync = settings.DispatchConsumersAsync,
            AutomaticRecoveryEnabled = settings.AutomaticRecoveryEnabled,
            NetworkRecoveryInterval = settings.NetworkRecoveryInterval,
            RequestedConnectionTimeout = settings.RequestedConnectionTimeout,
            RequestedHeartbeat = settings.RequestedHeartbeat,
            ContinuationTimeout = settings.ContinuationTimeout,
            HandshakeContinuationTimeout = settings.HandshakeContinuationTimeout,
            ClientProvidedName = settings.ConnectionName
        };
        connectionFactory.Ssl.Enabled = settings.Ssl;
        return connectionFactory;
    }
}
