namespace OmidBankSepah.MasterData.Domain.Contracts.Requests;

public sealed record ProductAvailabilityReservationItemReq(
    Guid ProductId,
    int RequestedQuantity,
    int AvailableQuantity);
