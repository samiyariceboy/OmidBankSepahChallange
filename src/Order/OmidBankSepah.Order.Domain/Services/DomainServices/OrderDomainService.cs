using OmidBankSepah.Order.Domain.Constraint;
using OmidBankSepah.Order.Domain.Contracts.Requests;
using OrderEntity = OmidBankSepah.Order.Domain.Entities.OrderRoot.Order;

namespace OmidBankSepah.Order.Domain.Services.DomainServices;

public sealed class OrderDomainService : IOrderDomainService
{
    public OrderEntity CreatePendingOrder(
        Guid customerId,
        string idempotencyKey,
        IReadOnlyCollection<OrderItemReq> items)
    {
        OrderAssert.NotNull(items, nameof(CreatePendingOrder), OrderErrors.OrderItemsRequired);
        OrderAssert.IsSuccess(items.Count > 0, [OrderErrors.OrderItemsRequired]);

        var order = OrderEntity.Create(customerId, idempotencyKey);
        foreach (var item in items)
        {
            OrderAssert.NotNull(item, nameof(CreatePendingOrder), OrderErrors.OrderItemsRequired);
            order.AddItem(item.ProductId, item.Price, item.Quantity);
        }

        return order;
    }
}
