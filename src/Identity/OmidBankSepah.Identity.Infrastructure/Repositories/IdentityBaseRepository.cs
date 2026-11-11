using OmidBankSepah.Identity.Domain.Repositories;
using OmidBankSepah.Identity.Infrastructure.EF.Context;
using Taday.Corelibrary.Domain.Entities;
using Taday.Corelibrary.Infrastucture.Repository;

namespace OmidBankSepah.Identity.Infrastructure.Repositories;

public abstract class IdentityBaseRepository<TEntity>
    : BaseRepository<OmidBankSepahIdentityDbContext, TEntity>, IIdentityBaseRepository<TEntity>
    where TEntity : class, IAggregateRoot
{
    protected IdentityBaseRepository(OmidBankSepahIdentityDbContext dbContext)
        : base(dbContext)
    {
    }
}
