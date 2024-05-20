using System.Diagnostics;
using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using NetVisionProc.Common.Data.Interfaces;
using NetVisionProc.Data;
using Serilog;
using ILogger = Serilog.ILogger;

namespace ClientApp.Setup
{
    public class DatabaseSetup
    {
        private const bool IsSqlLite = false;
        private readonly ILogger _logger = Log.ForContext<DatabaseSetup>();
        private readonly IHostEnvironment _env;
        private readonly IConfiguration _config;

        private const bool DeleteDbOnStartup = false;

        public DatabaseSetup(IHostEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
        }

        public void Configure(IServiceCollection services)
        {
            string connectionString;

            if (IsSqlLite)
            {
                connectionString = GenerateSqlLiteConnectionString();
            }
            else
            {
                connectionString = GetConnectionString();

                _logger.Information("Database connection string: {ConnectionString}", RemoveUserCredentialsFromConnectionString(connectionString));
            }

            services.AddDbContext<AppMainDbContext>(opt =>
            {
                if (!_env.IsProduction())
                {
                    if (_env.IsDevelopment())
                    {
                        opt.EnableSensitiveDataLogging(); // Enabled logging of SQL param values
                    }

                    opt.EnableDetailedErrors();
                }

                if (IsSqlLite)
                {
                    opt.UseSqlite(connectionString);
                }
                else
                {
                    opt.UseNpgsql(connectionString);
                    opt.UseExceptionProcessor();
                }
            });

            DbSeedSetup.Configure(services);
            
            services.AddDatabaseDeveloperPageExceptionFilter();
        }

        public async Task Initialize(IServiceProvider hostServices)
        {
            using var scope = hostServices.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var db = services.GetRequiredService<IEntityDbContext<AppMainDbContext>>();
                
                // if (dbContext.Database.IsNpgsql() || dbContext.Database.IsSqlServer())
                // {
                //     var connString = dbContext.Database.GetConnectionString();
                //     _logger.Information("Migration Started: {ConnectionString}", connString);
                //
                //     await RunMigrations(dbContext);
                // }
                
                if (DeleteDbOnStartup)
                {
                    await DropAndCreate(db);
                    ApplySeedForDb(new SeedRunner(services));   
                }
                else
                {
                    await db.BindedDbContext.Database.EnsureCreatedAsync();
                }

                return;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred creating the DB.");
                throw;
            }
        }

        private static string RemoveUserCredentialsFromConnectionString(string connectionString)
        {
            var resultParts = connectionString.Split(';')
                .Where(part => !part.StartsWith("User", StringComparison.OrdinalIgnoreCase) &&
                               !part.StartsWith("Password", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            var result = string.Join(";", resultParts);

            return result;
        }

        private string GenerateSqlLiteConnectionString()
        {
            var localAppDataFolder = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData,
                Environment.SpecialFolderOption.DoNotVerify);

            var databaseFolder = Path.Combine(localAppDataFolder, "EvoTestNN");
            Directory.CreateDirectory(databaseFolder);

            var databasePath = Path.Join(databaseFolder, $"{InfrastructureConst.Db.AppDbName}.db");

            _logger.Information("SqlLite Database Path: {SqlLiteDbPath}", databasePath);
            _logger.Warning("Using SqlLite Database");

            return $"Data Source={databasePath}";
        }

        private string GetConnectionString()
        {
            var connectionString = _config.GetConnectionString(InfrastructureConst.Db.AppDbName);
            Guard.Against.NullOrEmpty(connectionString, nameof(connectionString));

            _logger.Information("Using SQL Database");

            return connectionString;
        }

        private async Task RunMigrations(DbContext dbContext)
        {
            try
            {
                await dbContext.Database.MigrateAsync();

                _logger.Information("RunMigrations Finished");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "RunMigrations Failed");
                throw;
            }
        }
        
        private async Task DropAndCreate(IEntityDbContext<AppMainDbContext> db)
        {
            try
            {
                await db.BindedDbContext.Database.EnsureDeletedAsync();
                await db.BindedDbContext.Database.EnsureCreatedAsync();
                _logger.Information($"{nameof(DropAndCreate)} Finished");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{nameof(DropAndCreate)} Failed");
                throw;
            }
        }

        private void ApplySeedForDb(SeedRunner seedRunner)
        {
            Task.Run(async () =>
            {
                var timer = new Stopwatch();
                _logger.Information("ApplySeeds Started");
                timer.Start();

                await seedRunner.ApplyAll();

                timer.Stop();

                LogTimeElapsed("ApplySeeds.NewDb", timer.Elapsed);
            }).GetAwaiter().GetResult();
        }

        private void LogTimeElapsed(string source, TimeSpan timeElapsed)
        {
            _logger.Information("{Source} Finished - {Elapsed}", source, timeElapsed);
        }
    }
}