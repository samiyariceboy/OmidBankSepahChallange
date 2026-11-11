using Taday.Corelibrary.Domain.Entities;
using Taday.Corelibrary.Infrastucture.Repository;

namespace OmidBankSepah.Order.Domain.Repositories;

public interface IOrderBaseRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : class, IAggregateRoot
{
}
