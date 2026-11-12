using CoreRabbitMqSettings = Taday.Corelibrary.Common.Shared.RabbitMq;

namespace OmidBankSepah.Order.Infrastructure.Configuration;

public sealed class OrderRabbitMqSettings : CoreRabbitMqSettings
{
    public string ProductInventoryQueue { get; set; } = "Order.ProductInventory";
}
