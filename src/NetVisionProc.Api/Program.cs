using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using AutoFixture;
using Microsoft.AspNetCore.HttpOverrides;
using NetCore.AutoRegisterDi;
using NetVisionProc.Api.Setup;
using NetVisionProc.Application;
using NetVisionProc.Common.Data;
using NetVisionProc.Common.Data.Interfaces;
using NetVisionProc.Data;
using NetVisionProc.Domain;
using Serilog;
using Serilog.Debugging;
using Sieve.Models;
using Sieve.Services;

namespace NetVisionProc.Api
{
    public class Program
    {
        private const string AppName = "EvoCDCTest.Api";

        public static async Task Main(string[] args)
        {
            SelfLog.Enable(Console.Error);
            
            LoggingSetup.CreateBootstrapLogger();
            var logger = Log.ForContext<Program>();
            
            logger.Information($"Starting {AppName}");
            try
            {
                var builder = WebApplication.CreateBuilder(args);
                builder.Configuration
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("host.settings.json", optional: true, reloadOnChange: true);
                
                var host = builder.Host;
                var env = builder.Environment;
                var services = builder.Services;
                var config = builder.Configuration;

                AddConfigurations(services, config);

                var loggingSetup = new LoggingSetup(env, config);
                loggingSetup.Configure(host);

                var dbSetup = new DatabaseSetup(env, config);
                dbSetup.Configure(services);

                // DbChangeHandlerSetup.SetupDatabase(services, config);

                DbSeedSetup.Configure(services);

                ConfigureServices(services);

                SetupIntegrations(config);

                var corsSetup = new CorsSetup(config);
                var controllersSetup = new ApiSetup(env);

                services.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders =
                        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                    options.KnownNetworks.Clear();
                    options.KnownProxies.Clear();
                });
                corsSetup.Configure(services);
                services.AddHealthChecks()
                    .AddDbContextCheck<AppMainDbContext>();
                controllersSetup.Configure(services);

                var app = builder.Build();
                await dbSetup.Initialize(app.Services);

                app.UseForwardedHeaders();
                corsSetup.Configure(app);
                loggingSetup.Configure(app);
                app.UseHealthChecks("/admin/health-check");
                controllersSetup.Configure(app);

                await app.RunAsync();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, $"{AppName} terminated.");
            }
            finally
            {
                await Log.CloseAndFlushAsync();
            }
        }
        
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(
                typeof(BaseEntity).Assembly, // Domain
                typeof(HealthCheckTestService).Assembly); // Services

            services.AddScoped<IEntityOnlyQueryDbContext<AppMainDbContext>, EntityOnlyQueryDbContext<AppMainDbContext>>();
            services.AddScoped<IEntityDbContext<AppMainDbContext>, EntityDbContext<AppMainDbContext>>();
            
            services.AddScoped<IFixture, Fixture>();

            services.AddScoped<ISieveProcessor, SieveProcessor>();
            services.Configure<SieveOptions>(c =>
            {
                c.DefaultPageSize = CommonConst.TablePageSize;
                c.ThrowExceptions = true;
            });

            services.RegisterAssemblyPublicNonGenericClasses(
                typeof(HealthCheckTestService).Assembly)
                .AsPublicImplementedInterfaces(); // Transient by default
        }

        private static void SetupIntegrations(IConfiguration config)
        {
        }

        private static void AddConfigurations(IServiceCollection services, ConfigurationManager config)
        {
            services.AddOptions();
            services.Configure<JsonSerializerOptions>(c =>
            {
                c.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                c.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
        }
    }
}

