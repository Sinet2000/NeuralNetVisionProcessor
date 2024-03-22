using System;
using System.IO;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetVisionProc.AzureHub;
using NetVisionProc.AzureHub.Config;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

[assembly: FunctionsStartup(typeof(Startup))]
namespace NetVisionProc.AzureHub
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                // .WriteTo.Console(theme: SystemConsoleTheme.Colored)
                // .WriteTo.File("./durable-function-log.txt")
                .CreateLogger();

            var configurationBuilder = new ConfigurationBuilder()
                .AddEnvironmentVariables();

            var config = configurationBuilder
                .AddJsonFile(Path.Combine(builder.GetContext().ApplicationRootPath, "local.settings.json"), true, true)
                .AddJsonFile(Path.Combine(builder.GetContext().ApplicationRootPath, "host.json"), true, false)
                .Build();

            var azureHubConfig = new AzureHubConfig();
            config.GetSection(AzureHubConfig.SectionName).Bind(azureHubConfig);
            // builder.Services.ConfigureAzureHubDatabase<DbContext>(azureHubConfig);

            builder.Services.AddLogging(lb =>
            {
                lb.AddSerilog(Log.Logger, true);
            });

            builder.Services.AddSingleton(sp => new BlobServiceClient(azureHubConfig.BlobConnectionString));
            builder.Services.AddScoped(sp =>
            {
                var blobServiceClient = sp.GetRequiredService<BlobServiceClient>();
                const string containerName = "blob-py-test";
                return blobServiceClient.GetBlobContainerClient(containerName);
            });

            builder.Services.AddSingleton(sp => new TableServiceClient(azureHubConfig.TableConnectionString));
            builder.Services.AddScoped(sp =>
            {
                var tableServiceClient = sp.GetRequiredService<TableServiceClient>();
                const string tableName = "BlobTriggerTest";
                return tableServiceClient.GetTableClient(tableName);
            });

            const string queueName = "blob-py-test-queue";
            builder.Services.AddSingleton(sp => new QueueClient(azureHubConfig.QueueConnectionString, queueName));
        }
    }
}