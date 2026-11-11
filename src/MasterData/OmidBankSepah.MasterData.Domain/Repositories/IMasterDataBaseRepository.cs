using Taday.Corelibrary.Domain.Entities;
using Taday.Corelibrary.Infrastucture.Repository;

namespace OmidBankSepah.MasterData.Domain.Repositories;

public interface IMasterDataBaseRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : class, IAggregateRoot
{
}
