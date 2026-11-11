namespace OmidBankSepah.MasterData.Domain.Contracts.Requests;

public sealed record ProductAvailabilityReq(Guid ProductId, int Quantity);
