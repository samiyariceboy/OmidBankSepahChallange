using OrderEntity = OmidBankSepah.Order.Domain.Entities.OrderRoot.Order;

namespace OmidBankSepah.Order.Domain.Repositories;

public interface IOrderRepository : IOrderBaseRepository<OrderEntity>
{
    new Task<OrderEntity> AddAsync(
        OrderEntity entity,
        bool saveChanges = true,
        CancellationToken cancellationToken = default);

    new Task UpdateAsync(
        OrderEntity entity,
        bool saveChanges = true,
        CancellationToken cancellationToken = default);
}
