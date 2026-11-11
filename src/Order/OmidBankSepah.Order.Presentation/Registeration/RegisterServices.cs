using Taday.Corelibrary.Common.Shared;
using OmidBankSepah.Order.Domain.Repositories;
using OmidBankSepah.Order.Infrastructure.Repositories;

namespace OmidBankSepah.Order.Presentation.Registeration
{
    public static class RegisterServices
    {
        public static void RegisterLoggerServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped(typeof(IAppLogger<>), typeof(Taday.Corelibrary.Common.Shared.Logger<>));
        }

        public static void RegisterRepositories(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        }
    }
}
