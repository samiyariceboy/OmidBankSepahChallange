using CoreRabbitMqSettings = Taday.Corelibrary.Common.Shared.RabbitMq;

namespace OmidBankSepah.Wallet.Infrastructure.Configuration;

public sealed class WalletRabbitMqSettings : CoreRabbitMqSettings
{
    public string ProductInventoryQueue { get; set; } = "Wallet.ProductInventory";
}
