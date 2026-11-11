namespace OmidBankSepah.MasterData.Domain.Contracts.Responses;

public sealed record ProductAvailabilityRes(Guid ProductId, int AvailableQuantity);
