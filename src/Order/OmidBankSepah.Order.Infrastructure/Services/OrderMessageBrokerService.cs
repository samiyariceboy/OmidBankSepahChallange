using System.Text;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OmidBankSepah.IntegrationEvents.ProductInventory;
using OmidBankSepah.Order.Application.UseCases.ProductInventoryChanged;
using OmidBankSepah.Order.Infrastructure.Configuration;
using RabbitMQ.Client;
using Taday.Corelibrary.Common.Shared;
using Taday.Corelibrary.Infrastucture.Services.Broker;

namespace OmidBankSepah.Order.Infrastructure.Services;

public sealed class OrderMessageBrokerService(
    IAppLogger<RabbitMqBaseService> logger,
    ConnectionFactory connectionFactory,
    IServiceScopeFactory scopeFactory,
    OrderRabbitMqSettings rabbitMqSettings)
    : RabbitMqBaseService(logger, connectionFactory, rabbitMqSettings), IHostedService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    protected override Task Subscribe()
    {
        return SubscribeQueue(rabbitMqSettings.ProductInventoryQueue, OnProductInventoryChanged);
    }

    private async Task<bool> OnProductInventoryChanged(
        byte[] messageBody,
        string exchange,
        string routingKey)
    {
        try
        {
            var message = JsonSerializer.Deserialize<ProductInventoryChangedMessage>(
                Encoding.UTF8.GetString(messageBody),
                _jsonSerializerOptions);

            if (message is null)
            {
                logger.Log(LogLevel.Error, "Product inventory message is invalid", nameof(OnProductInventoryChanged));
                return false;
            }

            using var scope = scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(new ProductInventoryChangedCommand(message));
            return true;
        }
        catch (Exception exception)
        {
            logger.Log(
                LogLevel.Error,
                "Error consuming product inventory message: {ErrorMessage}",
                nameof(OnProductInventoryChanged),
                exception.Message);
            return false;
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Start(cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Dispose();
        return Task.CompletedTask;
    }
}
