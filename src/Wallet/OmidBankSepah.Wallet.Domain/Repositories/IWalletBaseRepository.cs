using Ardalis.Specification;
using Taday.Corelibrary.Domain.Entities;

namespace OmidBankSepah.Wallet.Domain.Repositories;

public interface IWalletBaseRepository<TEntity> : IRepositoryBase<TEntity>
    where TEntity : class, IAggregateRoot
{
}
