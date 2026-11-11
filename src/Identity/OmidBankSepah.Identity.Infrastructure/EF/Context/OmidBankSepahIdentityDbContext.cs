using MediatR;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Taday.Corelibrary.Common.Shared;
using Taday.Corelibrary.Infrastucture.Contexts;

namespace OmidBankSepah.Identity.Infrastructure.EF.Context;
public sealed class OmidBankSepahIdentityDbContext : BaseIdentityDbContext<
    OmidBankSepahIdentityDbContext,
    IdentityUser<Guid>,
    IdentityRole<Guid>,
    Guid,
    IdentityUserClaim<Guid>,
    IdentityUserRole<Guid>,
    IdentityUserLogin<Guid>,
    IdentityRoleClaim<Guid>,
    IdentityUserToken<Guid>>
{
    public OmidBankSepahIdentityDbContext(
        DbContextOptions<OmidBankSepahIdentityDbContext> options,
        IAppLogger<OmidBankSepahIdentityDbContext> logger,
        IMediator mediator)
        : base(options, logger, mediator)
    {
    }

    protected override Assembly GetEntityAssemblies()
    {
        return Assembly.Load("OmidBankSepah.Identity.Domain");
    }

    protected override Assembly GetEntityConfigurationAssemblies()
    {
        return typeof(OmidBankSepahIdentityDbContext).Assembly;
    }
}
