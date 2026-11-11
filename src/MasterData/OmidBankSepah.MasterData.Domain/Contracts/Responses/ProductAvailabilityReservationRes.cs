namespace OmidBankSepah.MasterData.Domain.Contracts.Responses;

public sealed record ProductAvailabilityReservationRes(
    bool IsReserved,
    IReadOnlyList<ProductAvailabilityReservationItemRes> UnavailableProducts);
