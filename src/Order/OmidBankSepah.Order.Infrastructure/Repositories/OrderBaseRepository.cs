using OmidBankSepah.Order.Domain.Repositories;
using OmidBankSepah.Order.Infrastructure.Context;
using Taday.Corelibrary.Domain.Entities;
using Taday.Corelibrary.Infrastucture.Repository;

namespace OmidBankSepah.Order.Infrastructure.Repositories;

public abstract class OrderBaseRepository<TEntity>
    : BaseRepository<OmidBankSepahOrderDbContext, TEntity>, IOrderBaseRepository<TEntity>
    where TEntity : class, IAggregateRoot
{
    protected OrderBaseRepository(OmidBankSepahOrderDbContext dbContext)
        : base(dbContext)
    {
    }
}
