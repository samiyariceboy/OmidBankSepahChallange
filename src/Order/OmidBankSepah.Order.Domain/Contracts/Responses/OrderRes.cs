using OmidBankSepah.Order.Domain.Enums;

namespace OmidBankSepah.Order.Domain.Contracts.Responses;

public sealed record OrderItemRes(
    Guid Id,
    Guid ProductId,
    decimal Price,
    int Quantity,
    decimal TotalPrice);

public sealed record OrderRes(
    Guid Id,
    Guid CustomerId,
    OrderStatus Status,
    decimal TotalAmount,
    DateTime CreatedAt,
    string? RejectionReason,
    IReadOnlyCollection<OrderItemRes> Items);
