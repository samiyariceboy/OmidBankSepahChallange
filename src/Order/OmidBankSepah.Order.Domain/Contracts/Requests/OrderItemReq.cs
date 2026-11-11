namespace OmidBankSepah.Order.Domain.Contracts.Requests;

public sealed record OrderItemReq(Guid ProductId, decimal Price, int Quantity);
