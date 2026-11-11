using Ardalis.Specification;
using Taday.Corelibrary.Domain.Entities;

namespace OmidBankSepah.Order.Domain.Repositories;

public interface IOrderBaseRepository<TEntity> : IRepositoryBase<TEntity>
    where TEntity : class, IAggregateRoot
{
}
