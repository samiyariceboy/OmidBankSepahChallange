namespace OmidBankSepah.IntegrationEvents.ProductInventory;

public sealed record ProductInventoryChangedMessage(
    Guid EventId,
    Guid? OrderId,
    string ChangeType,
    IReadOnlyCollection<ProductInventoryChangedItem> Items,
    DateTimeOffset OccurredAtUtc);

public sealed record ProductInventoryChangedItem(Guid ProductId, int QuantityDelta);
