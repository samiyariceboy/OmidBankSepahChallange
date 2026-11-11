using OmidBankSepah.MasterData.Domain.Repositories;
using OmidBankSepah.MasterData.Infrastructure.Context;
using Taday.Corelibrary.Domain.Entities;
using Taday.Corelibrary.Infrastucture.Repository;

namespace OmidBankSepah.MasterData.Infrastructure.Repositories;

public abstract class MasterDataBaseRepository<TEntity>
    : BaseRepository<OmidBankSepahMasterDataDbContext, TEntity>, IMasterDataBaseRepository<TEntity>
    where TEntity : class, IAggregateRoot
{
    protected MasterDataBaseRepository(OmidBankSepahMasterDataDbContext dbContext)
        : base(dbContext)
    {
    }
}
