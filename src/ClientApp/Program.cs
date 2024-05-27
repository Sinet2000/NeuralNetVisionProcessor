using System.Net.Mime;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using AutoFixture;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using ClientApp.Components;
using ClientApp.Config;
using ClientApp.Setup;
using Microsoft.AspNetCore.ResponseCompression;
using NetCore.AutoRegisterDi;
using NetVisionProc.Application;
using Serilog;
using Sieve.Models;
using Sieve.Services;

namespace ClientApp
{
    public class Program
    {
        private const string AppName = "ClientApp";
        
        public static async Task Main(string[] args)
        {
            LoggingSetup.CreateBootstrapLogger();
            
            try
            {
                var builder = WebApplication.CreateBuilder(args);
                
                var host = builder.Host;
                var env = builder.Environment;
                var services = builder.Services;
                var config = builder.Configuration;
                
                AddConfigurations(services, config);
                
                var azureHubConfig = new AzureHubConfig();
                config.GetSection(AzureHubConfig.SectionName).Bind(azureHubConfig);
                
                var loggingSetup = new LoggingSetup(env, config);
                loggingSetup.Configure(host);
                
                
                ConfigureServices(services, azureHubConfig);
                
                //var corsSetup = new CorsSetup(config);
                //corsSetup.Configure(services);
                
                // services.AddHealthChecks()
                //     .AddDbContextCheck<AppMainDbContext>();

                services.ConfigureMudBlazor();

                var app = builder.Build();
                
                // corsSetup.Configure(app);
                loggingSetup.Configure(app);
                // app.UseHealthChecks("/admin/health-check");
                
                app.UseResponseCompression();
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Error", createScopeForErrors: true);
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();

                app.UseStaticFiles();
                app.UseAntiforgery();

                app.MapRazorComponents<App>()
                    .AddInteractiveServerRenderMode();

                await app.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal(ex, $"{AppName} terminated.");
            }
            finally
            {
                await Log.CloseAndFlushAsync();
            }
        }
        
        private static void ConfigureServices(IServiceCollection services, AzureHubConfig azureHubConfig)
        {
            services.AddAutoMapper(
                typeof(AzureHubConfig).Assembly,
                typeof(HealthCheckTestService).Assembly); // Application
            
            services.RegisterAssemblyPublicNonGenericClasses(
                    typeof(HealthCheckTestService).Assembly)
                .AsPublicImplementedInterfaces(); // Transient by default
            
            services.AddResponseCompression(options => {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new [] {
                    MediaTypeNames.Application.Octet,
                });
            });
            
            services.AddSingleton(sp => new BlobServiceClient(azureHubConfig.BlobConnectionString));
            services.AddScoped(sp =>
            {
                var blobServiceClient = sp.GetRequiredService<BlobServiceClient>();
                return blobServiceClient.GetBlobContainerClient(azureHubConfig.BlobContainerName);
            });

            services.AddSingleton(sp => new TableServiceClient(azureHubConfig.TableConnectionString));
            services.AddScoped(sp =>
            {
                var tableServiceClient = sp.GetRequiredService<TableServiceClient>();
                return tableServiceClient.GetTableClient(azureHubConfig.TableStorageName);
            });
            
            // https://learn.microsoft.com/en-us/azure/storage/queues/storage-tutorial-queues?tabs=environment-variable-windows
            services.AddSingleton(sp =>
                new QueueClient(azureHubConfig.QueueConnectionString, azureHubConfig.QueueName));
        }
        
        private static void AddConfigurations(IServiceCollection services, ConfigurationManager config)
        {
            services.AddOptions();
            services.Configure<AzureHubConfig>(config.GetSection(AzureHubConfig.SectionName));
            
            services.Configure<JsonSerializerOptions>(c =>
            {
                c.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                c.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
        }
    }
}
