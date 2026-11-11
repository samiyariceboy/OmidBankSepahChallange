using OrderEntity = OmidBankSepah.Order.Domain.Entities.OrderRoot.Order;

namespace OmidBankSepah.Order.Domain.Repositories;

public interface IOrderRepository : IOrderBaseRepository<OrderEntity>
{
}
