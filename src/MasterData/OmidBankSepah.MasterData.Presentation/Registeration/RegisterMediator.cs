using System.Reflection;

namespace OmidBankSepah.MasterData.Presentation.Registeration
{
    public static class RegisterMediator
    {
        public static IServiceCollection RegisterMediatR(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(assemblies);
                cfg.Lifetime = ServiceLifetime.Scoped;
            });

            return services;
        }
    }
}
