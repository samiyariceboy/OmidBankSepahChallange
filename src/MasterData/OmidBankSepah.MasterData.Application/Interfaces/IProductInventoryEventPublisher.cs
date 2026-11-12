using OmidBankSepah.IntegrationEvents.ProductInventory;

namespace OmidBankSepah.MasterData.Application.Interfaces;

public interface IProductInventoryEventPublisher
{
    Task TryPublishAsync(
        ProductInventoryChangedMessage message,
        CancellationToken cancellationToken);
}
