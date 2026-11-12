using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OmidBankSepah.Wallet.Infrastructure.Configuration;
using OmidBankSepah.Wallet.Infrastructure.Services;
using RabbitMQ.Client;
using Taday.Corelibrary.Common.Shared;
using Taday.Corelibrary.Infrastucture.Services.Broker;

namespace OmidBankSepah.Wallet.Presentation.Registeration;

public static class WalletRabbitMqRegistration
{
    public static IServiceCollection AddWalletRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = configuration.GetSection("RabbitMq").Get<WalletRabbitMqSettings>()
            ?? throw new InvalidOperationException("RabbitMq configuration is required.");
        settings.GetRabbitMqQueueConfig(settings.ProductInventoryQueue);

        services.AddSingleton<
            IAppLogger<RabbitMqBaseService>,
            Taday.Corelibrary.Common.Shared.Logger<RabbitMqBaseService>>();
        services.AddSingleton(settings);
        services.AddSingleton(CreateConnectionFactory(settings));
        services.AddSingleton<WalletMessageBrokerService>();
        services.AddHostedService(provider =>
            provider.GetRequiredService<WalletMessageBrokerService>());

        return services;
    }

    private static ConnectionFactory CreateConnectionFactory(WalletRabbitMqSettings settings)
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
