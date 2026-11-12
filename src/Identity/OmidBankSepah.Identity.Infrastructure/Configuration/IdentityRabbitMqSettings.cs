using CoreRabbitMqSettings = Taday.Corelibrary.Common.Shared.RabbitMq;

namespace OmidBankSepah.Identity.Infrastructure.Configuration;

public sealed class IdentityRabbitMqSettings : CoreRabbitMqSettings
{
    public string ProductInventoryQueue { get; set; } = "Identity.ProductInventory";
}
