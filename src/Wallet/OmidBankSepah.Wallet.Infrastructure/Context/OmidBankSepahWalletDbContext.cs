using MediatR;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Taday.Corelibrary.Common.Shared;
using Taday.Corelibrary.Infrastucture.Contexts;
using OmidBankSepah.Wallet.Domain.Entities.WalletRoot.Aggregates;

namespace OmidBankSepah.Wallet.Infrastructure.Context;
public sealed class OmidBankSepahWalletDbContext : BaseDbContext<OmidBankSepahWalletDbContext>
{
    public OmidBankSepahWalletDbContext(
        DbContextOptions<OmidBankSepahWalletDbContext> options,
        IAppLogger<OmidBankSepahWalletDbContext> logger,
        IMediator mediator)
        : base(options, logger, mediator)
    {
    }

    protected override Assembly GetEntityAssemblies()
    {
        return typeof(WalletTransaction).Assembly;
    }

    protected override Assembly GetEntityConfigurationAssemblies()
    {
        return typeof(OmidBankSepahWalletDbContext).Assembly;
    }
}
