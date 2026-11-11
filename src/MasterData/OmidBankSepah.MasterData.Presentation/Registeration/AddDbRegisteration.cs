using Microsoft.EntityFrameworkCore;
using OmidBankSepah.MasterData.Infrastructure.Context;

namespace OmidBankSepah.Order.Presentation.Registeration
{
    public static class AddDbRegisteration
    {
        public static void RegisterDb(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<OmidBankSepahMasterDataDbContext>
           (options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"),
            x => x.UseNetTopologySuite()));
        }
    }
}
