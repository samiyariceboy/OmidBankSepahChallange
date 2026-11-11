using OmidBankSepah.Order.Domain.Contracts.Requests;
using OrderEntity = OmidBankSepah.Order.Domain.Entities.OrderRoot.Order;

namespace OmidBankSepah.Order.Domain.Services.DomainServices;

public interface IOrderDomainService
{
    OrderEntity CreatePendingOrder(
        Guid customerId,
        string idempotencyKey,
        IReadOnlyCollection<OrderItemReq> items);
}
