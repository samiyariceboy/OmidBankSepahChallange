using CoreRabbitMqSettings = Taday.Corelibrary.Common.Shared.RabbitMq;

namespace OmidBankSepah.MasterData.Infrastructure.Configuration;

public sealed class MasterDataRabbitMqSettings : CoreRabbitMqSettings
{
    public string ProductInventoryQueue { get; set; } = "MasterData.ProductInventory";
}
