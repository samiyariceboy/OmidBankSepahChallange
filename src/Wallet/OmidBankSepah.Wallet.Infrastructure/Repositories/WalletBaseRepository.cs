using Taday.Corelibrary.Domain.Entities;
using OmidBankSepah.Wallet.Domain.Repositories;
using Taday.Corelibrary.Infrastucture.Repository;
using OmidBankSepah.Wallet.Infrastructure.Context;

namespace OmidBankSepah.Wallet.Infrastructure.Repositories;
public abstract class WalletBaseRepository<TEntity>
    : BaseRepository<OmidBankSepahWalletDbContext, TEntity>, IWalletBaseRepository<TEntity>
    where TEntity : class, IAggregateRoot
{
    protected WalletBaseRepository(OmidBankSepahWalletDbContext dbContext)
        : base(dbContext)
    {
    }
}
