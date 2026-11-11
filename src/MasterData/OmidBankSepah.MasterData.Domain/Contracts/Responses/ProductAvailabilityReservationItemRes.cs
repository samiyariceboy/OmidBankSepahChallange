namespace OmidBankSepah.MasterData.Domain.Contracts.Responses;

public sealed record ProductAvailabilityReservationItemRes(
    Guid ProductId,
    int RequestedQuantity,
    int AvailableQuantity);
