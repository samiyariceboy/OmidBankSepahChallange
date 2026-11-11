namespace OmidBankSepah.Order.Application.Contracts.Responses;

public sealed record UnavailableProductAvailabilityRes(
    Guid ProductId,
    int RequestedQuantity,
    int AvailableQuantity);

public sealed record ProductAvailabilityReservationRes(
    bool IsReserved,
    IReadOnlyCollection<UnavailableProductAvailabilityRes> UnavailableItems);
