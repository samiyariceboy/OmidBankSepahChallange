using OmidBankSepah.Order.Domain.Repositories;
using OmidBankSepah.Order.Infrastructure.Context;
using OrderEntity = OmidBankSepah.Order.Domain.Entities.OrderRoot.Order;

namespace OmidBankSepah.Order.Infrastructure.Repositories;

public sealed class OrderRepository(OmidBankSepahOrderDbContext dbContext)
    : OrderBaseRepository<OrderEntity>(dbContext), IOrderRepository
{
}
