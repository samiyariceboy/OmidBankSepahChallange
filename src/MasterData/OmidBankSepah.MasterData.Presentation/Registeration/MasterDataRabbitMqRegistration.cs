using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OmidBankSepah.MasterData.Infrastructure.Configuration;
using OmidBankSepah.MasterData.Infrastructure.Services;
using OmidBankSepah.MasterData.Application.Interfaces;
using Taday.Corelibrary.Common.Shared;
using Taday.Corelibrary.Infrastucture.Services.Broker;

namespace OmidBankSepah.MasterData.Presentation.Registeration;
public static class MasterDataRabbitMqRegistration
{
    public static IServiceCollection AddMasterDataRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = configuration.GetSection("RabbitMq").Get<MasterDataRabbitMqSettings>()
            ?? throw new InvalidOperationException("RabbitMq configuration is required.");
        settings.GetRabbitMqQueueConfig(settings.ProductInventoryQueue);

        services.AddSingleton<
            IAppLogger<RabbitMqBaseService>,
            Taday.Corelibrary.Common.Shared.Logger<RabbitMqBaseService>>();
        services.AddSingleton(settings);
        services.AddSingleton(CreateConnectionFactory(settings));
        services.AddSingleton<MasterDataMessageBrokerService>();

        services.AddSingleton<IProductInventoryEventPublisher>(provider =>
            provider.GetRequiredService<MasterDataMessageBrokerService>());

        services.AddHostedService(provider =>
            provider.GetRequiredService<MasterDataMessageBrokerService>());

        return services;
    }

    private static ConnectionFactory CreateConnectionFactory(MasterDataRabbitMqSettings settings)
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
