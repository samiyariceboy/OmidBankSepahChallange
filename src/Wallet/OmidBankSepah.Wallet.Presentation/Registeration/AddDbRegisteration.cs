using Microsoft.EntityFrameworkCore;
using OmidBankSepah.Wallet.Infrastructure.Context;

namespace OmidBankSepah.Wallet.Presentation.Registeration
{
    public static class AddDbRegisteration
    {
        public static void RegisterDb(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<OmidBankSepahWalletDbContext>
           (options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"),
            x => x.UseNetTopologySuite()));
        }
    }
}
