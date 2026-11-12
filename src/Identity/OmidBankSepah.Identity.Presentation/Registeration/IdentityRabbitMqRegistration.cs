using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OmidBankSepah.Identity.Infrastructure.Configuration;
using OmidBankSepah.Identity.Infrastructure.Services;
using RabbitMQ.Client;
using Taday.Corelibrary.Common.Shared;
using Taday.Corelibrary.Infrastucture.Services.Broker;

namespace OmidBankSepah.Identity.Presentation.Registeration;

public static class IdentityRabbitMqRegistration
{
    public static IServiceCollection AddIdentityRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = configuration.GetSection("RabbitMq").Get<IdentityRabbitMqSettings>()
            ?? throw new InvalidOperationException("RabbitMq configuration is required.");
        settings.GetRabbitMqQueueConfig(settings.ProductInventoryQueue);

        services.TryAddScoped(
            typeof(IAppLogger<>),
            typeof(Taday.Corelibrary.Common.Shared.Logger<>));
        services.AddSingleton<
            IAppLogger<RabbitMqBaseService>,
            Taday.Corelibrary.Common.Shared.Logger<RabbitMqBaseService>>();
        services.AddSingleton(settings);
        services.AddSingleton(CreateConnectionFactory(settings));
        services.AddSingleton<IdentityMessageBrokerService>();
        services.AddHostedService(provider =>
            provider.GetRequiredService<IdentityMessageBrokerService>());

        return services;
    }

    private static ConnectionFactory CreateConnectionFactory(IdentityRabbitMqSettings settings)
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
