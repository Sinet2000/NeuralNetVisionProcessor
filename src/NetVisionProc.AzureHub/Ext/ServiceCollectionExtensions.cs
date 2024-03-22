using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetVisionProc.AzureHub.Config;

namespace NetVisionProc.AzureHub.Ext
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureAzureHubDatabase<TContext>(this IServiceCollection services, AzureHubConfig azureHubConfig)
        where TContext: DbContext
        {
            services.AddDbContextFactory<TContext>(options =>
            {
                options.UseSqlServer(
                        azureHubConfig.DbConnectionString,
                        sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(10);
                            sqlOptions.CommandTimeout(300);
                            sqlOptions.MinBatchSize(1);
                            sqlOptions.MaxBatchSize(100);
                        })
                    .EnableSensitiveDataLogging(false);
            });
        }
    }
}