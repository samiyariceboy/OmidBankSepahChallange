using Ardalis.Specification;
using Taday.Corelibrary.Domain.Entities;

namespace OmidBankSepah.MasterData.Domain.Repositories;

public interface IMasterDataBaseRepository<TEntity> : IRepositoryBase<TEntity>
    where TEntity : class, IAggregateRoot
{
}
