namespace OmidBankSepah.Order.Application.Contracts.Requests;

public sealed record CreateOrderReq(
    Guid CustomerId,
    string? IdempotencyKey,
    IReadOnlyCollection<CreateOrderItemReq>? Items);

public sealed record CreateOrderItemReq(Guid ProductId, decimal Price, int Quantity);
