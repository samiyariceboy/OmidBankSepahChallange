using Taday.Corelibrary.Common.Shared;
using OmidBankSepah.Wallet.Domain.Repositories;
using OmidBankSepah.Wallet.Infrastructure.Repositories;

namespace OmidBankSepah.Wallet.Presentation.Registeration
{
    public static class RegisterServices
    {
        public static void RegisterLoggerServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped(typeof(IAppLogger<>), typeof(Taday.Corelibrary.Common.Shared.Logger<>));
        }

        public static void RegisterRepositories(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IWalletRepository, WalletRepository>();
        }
    }
}