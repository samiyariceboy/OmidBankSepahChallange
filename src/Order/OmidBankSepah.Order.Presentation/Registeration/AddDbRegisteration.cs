using Microsoft.EntityFrameworkCore;
using OmidBankSepah.Order.Infrastructure.Context;

namespace OmidBankSepah.Order.Presentation.Registeration
{
    public static class AddDbRegisteration
    {
        public static void RegisterDb(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<OmidBankSepahOrderDbContext>
           (options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"),
            x => x.UseNetTopologySuite()));
        }
    }
}
