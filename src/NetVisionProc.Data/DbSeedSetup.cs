using Microsoft.Extensions.DependencyInjection;

namespace NetVisionProc.Data
{
    public static class DbSeedSetup
    {
        public static void Configure(IServiceCollection services)
        {
            AddFakers(services);

            AddSeeds(services);
        }

        private static void AddFakers(IServiceCollection services)
        {
            // services.AddScoped<PersonFaker>();
        }

        private static void AddSeeds(IServiceCollection services)
        {
            // services.AddScoped<CountrySeed>();
        }
    }
}
