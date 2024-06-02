using System.IO;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetVisionProc.AzureHub;
using NetVisionProc.AzureHub.Config;
using NetVisionProc.AzureHub.Utils;
using Serilog;

[assembly: FunctionsStartup(typeof(Startup))]
namespace NetVisionProc.AzureHub
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .CreateLogger();

            var configurationBuilder = new ConfigurationBuilder()
                .AddEnvironmentVariables();

            var config = configurationBuilder
                .AddJsonFile(Path.Combine(builder.GetContext().ApplicationRootPath, "local.settings.json"), true, true)
                .AddJsonFile(Path.Combine(builder.GetContext().ApplicationRootPath, "host.json"), true, false)
                .Build();

            var azureHubConfig = new AzureHubConfig();
            config.GetSection(AzureHubConfig.SectionName).Bind(azureHubConfig);
            builder.Services.AddSingleton(azureHubConfig);

            var neuralVisionSystemConfig = new NeuralVisionSystemConfig();
            config.GetSection(NeuralVisionSystemConfig.SectionName).Bind(neuralVisionSystemConfig);
            builder.Services.AddSingleton(neuralVisionSystemConfig);
            
            // builder.Services.ConfigureAzureHubDatabase<DbContext>(azureHubConfig);

            builder.Services.AddLogging(lb =>
            {
                lb.AddSerilog(Log.Logger, true);
            });

            builder.Services.AddSingleton(sp => new BlobServiceClient(azureHubConfig.BlobConnectionString));
            builder.Services.AddScoped(sp =>
            {
                var blobServiceClient = sp.GetRequiredService<BlobServiceClient>();
                return blobServiceClient.GetBlobContainerClient(azureHubConfig.BlobContainerName);
            });

            builder.Services.AddSingleton(sp => new TableServiceClient(azureHubConfig.TableConnectionString));
            builder.Services.AddScoped(sp =>
            {
                var tableServiceClient = sp.GetRequiredService<TableServiceClient>();
                return tableServiceClient.GetTableClient(azureHubConfig.TableStorageName);
            });

            builder.Services.AddSingleton(sp =>
                new QueueClient(azureHubConfig.QueueConnectionString, azureHubConfig.QueueName));
            
            // Create blob containers if doesn't exist
            AzureBlobUtil
                .CheckAndCreateBlobContainer(azureHubConfig.BlobConnectionString, azureHubConfig.BlobContainerName)
                .Wait();
            
            AzureBlobUtil
                .CheckAndCreateBlobContainer(azureHubConfig.BlobConnectionString, azureHubConfig.ProcessedBlobsContainerName)
                .Wait();
        }
    }
}