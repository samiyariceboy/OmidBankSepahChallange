using OmidBankSepah.Order.Domain.Repositories;
using OmidBankSepah.Order.Infrastructure.Context;
using OrderEntity = OmidBankSepah.Order.Domain.Entities.OrderRoot.Order;

namespace OmidBankSepah.Order.Infrastructure.Repositories;

public sealed class OrderRepository(OmidBankSepahOrderDbContext dbContext)
    : OrderBaseRepository<OrderEntity>(dbContext), IOrderRepository
{
    public new async Task<OrderEntity> AddAsync(
        OrderEntity entity,
        bool saveChanges = true,
        CancellationToken cancellationToken = default)
    {
        dbContext.Set<OrderEntity>().Add(entity);

        if (saveChanges)
        {
            await dbContext.SaveChangesAsync(
                acceptAllChangesOnSuccess: true,
                cancellationToken);
        }

        return entity;
    }

    public new async Task UpdateAsync(
        OrderEntity entity,
        bool saveChanges = true,
        CancellationToken cancellationToken = default)
    {
        dbContext.Set<OrderEntity>().Update(entity);

        if (saveChanges)
        {
            await dbContext.SaveChangesAsync(
                acceptAllChangesOnSuccess: true,
                cancellationToken);
        }
    }
}
