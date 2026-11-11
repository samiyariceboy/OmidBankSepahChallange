using OmidBankSepah.Wallet.Domain.Entities.WalletRoot;
using OmidBankSepah.Wallet.Domain.Repositories;
using OmidBankSepah.Wallet.Infrastructure.Context;

namespace OmidBankSepah.Wallet.Infrastructure.Repositories;

public sealed class WalletRepository(OmidBankSepahWalletDbContext dbContext)
    : WalletBaseRepository<Wallets>(dbContext), IWalletRepository
{
}
