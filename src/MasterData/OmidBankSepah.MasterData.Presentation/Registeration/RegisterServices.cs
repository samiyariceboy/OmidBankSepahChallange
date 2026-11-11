using Taday.Corelibrary.Common.Shared;
using OmidBankSepah.MasterData.Domain.Services.DomainServices;
using OmidBankSepah.MasterData.Domain.Repositories;
using OmidBankSepah.MasterData.Infrastructure.Repositories;

namespace OmidBankSepah.MasterData.Presentation.Registeration
{
    public static class RegisterServices
    {
        public static void RegisterLoggerServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped(typeof(IAppLogger<>), typeof(Taday.Corelibrary.Common.Shared.Logger<>));
        }

        public static void RegisterRepositories(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IProductsRepository, ProductsRepository>();
            builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
            builder.Services.AddScoped<IProductInventoryRepository, ProductInventoryRepository>();
            builder.Services.AddScoped<IProductInventoryReservationRepository, ProductInventoryReservationRepository>();
        }
    }
}
