using MediatR;
using System.Text;
using RabbitMQ.Client;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Taday.Corelibrary.Common.Shared;
using Taday.Corelibrary.Infrastucture.Services.Broker;
using OmidBankSepah.IntegrationEvents.ProductInventory;
using OmidBankSepah.MasterData.Infrastructure.Configuration;
using OmidBankSepah.MasterData.Application.UseCases.ProductInventoryChanged;
using OmidBankSepah.MasterData.Application.Interfaces;

namespace OmidBankSepah.MasterData.Infrastructure.Services;
public sealed class MasterDataMessageBrokerService(
    IAppLogger<RabbitMqBaseService> logger,
    ConnectionFactory connectionFactory,
    IServiceScopeFactory scopeFactory,
    MasterDataRabbitMqSettings rabbitMqSettings)
    : RabbitMqBaseService(logger, connectionFactory, rabbitMqSettings),
        IProductInventoryEventPublisher,
        IHostedService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    protected override Task Subscribe()
    {
        return SubscribeQueue(rabbitMqSettings.ProductInventoryQueue, OnProductInventoryChanged);
    }

    public async Task TryPublishAsync(
        ProductInventoryChangedMessage message,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            await PublishAsync(message, rabbitMqSettings.ProductInventoryQueue);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.Log(
                LogLevel.Error,
                "Failed to publish product inventory message: {ErrorMessage}",
                nameof(TryPublishAsync),
                exception.Message);
        }
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
            var bus = scope.ServiceProvider.GetRequiredService<IMediator>();
            await bus.Send(new ProductInventoryChangedCommand(message));
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
