using MediatR;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Taday.Corelibrary.Common.Shared;
using Taday.Corelibrary.Infrastucture.Contexts;
using OmidBankSepah.MasterData.Domain.Entities.ProductRoot;

namespace OmidBankSepah.MasterData.Infrastructure.Context;
public sealed class OmidBankSepahMasterDataDbContext : BaseDbContext<OmidBankSepahMasterDataDbContext>
{
    public OmidBankSepahMasterDataDbContext(
        DbContextOptions<OmidBankSepahMasterDataDbContext> options,
        IAppLogger<OmidBankSepahMasterDataDbContext> logger,
        IMediator mediator)
        : base(options, logger, mediator)
    {
    }

    protected override Assembly GetEntityAssemblies()
    {
        return typeof(Products).Assembly;
    }

    protected override Assembly GetEntityConfigurationAssemblies()
    {
        return typeof(OmidBankSepahMasterDataDbContext).Assembly;
    }
}
