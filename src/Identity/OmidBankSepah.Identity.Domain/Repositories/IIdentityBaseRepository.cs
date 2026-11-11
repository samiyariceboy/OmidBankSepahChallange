using Ardalis.Specification;
using Taday.Corelibrary.Domain.Entities;

namespace OmidBankSepah.Identity.Domain.Repositories;

public interface IIdentityBaseRepository<TEntity> : IRepositoryBase<TEntity>
    where TEntity : class, IAggregateRoot
{
}
