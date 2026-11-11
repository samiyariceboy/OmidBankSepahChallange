using MediatR;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Taday.Corelibrary.Common.Shared;
using Taday.Corelibrary.Infrastucture.Contexts;
using OrderEntity = OmidBankSepah.Order.Domain.Entities.OrderRoot.Order;

namespace OmidBankSepah.Order.Infrastructure.Context;
public sealed class OmidBankSepahOrderDbContext : BaseDbContext<OmidBankSepahOrderDbContext>
{
    public OmidBankSepahOrderDbContext(
        DbContextOptions<OmidBankSepahOrderDbContext> options,
        IAppLogger<OmidBankSepahOrderDbContext> logger,
        IMediator mediator)
        : base(options, logger, mediator)
    {
    }

    protected override Assembly GetEntityAssemblies()
    {
        return typeof(OrderEntity).Assembly;
    }

    protected override Assembly GetEntityConfigurationAssemblies()
    {
        return typeof(OmidBankSepahOrderDbContext).Assembly;
    }
}
