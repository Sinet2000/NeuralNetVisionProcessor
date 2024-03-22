using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using ILogger = Serilog.ILogger;

namespace NetVisionProc.Api.Setup
{
    public class LoggingSetup
    {
        private const string ConsoleOutputTemplate = "[{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";
        private const string FileOutputTemplate = "{Timestamp:yyyy.MM.dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

        private readonly IHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly ILogger _logger = Log.ForContext<LoggingSetup>();
        private readonly List<string> _logFiles = [];
        
        public LoggingSetup(IHostEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
        }
        
        public static void CreateBootstrapLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate: ConsoleOutputTemplate,
                    theme: AnsiConsoleTheme.Code)
                .CreateBootstrapLogger();
        }
        
        public void Configure(IHostBuilder builder)
        {
            builder.UseSerilog((context, services, configuration) =>
            {
                if (!_env.IsProduction())
                {
                    configuration.WriteTo.Console(
                        outputTemplate: ConsoleOutputTemplate,
                        theme: AnsiConsoleTheme.Code);
                }

                if (_env.IsDevelopment())
                {
                    configuration.WriteTo.Conditional(
                        c => c.Level >= LogEventLevel.Warning,
                        cs => EnableLogToFile(cs, "errors.txt"));
                    
                    configuration.WriteTo.Seq("http://localhost:5341");
                }

                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext();
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging();

            int i = 0;
            foreach (var path in _logFiles)
            {
                i++;
                _logger.Information("LogFile[{Index}]: {LogFile}", i, path);
            }
        }

        private void EnableLogToFile(LoggerSinkConfiguration loggerSink, string fileName)
        {
            string? tempPath = Environment.GetEnvironmentVariable("TEMP");

            if (string.IsNullOrEmpty(tempPath))
            {
                _logger.Warning("Environment variable TEMP is not set => could not enable file logs");
            }
            else
            {
                string path = Path.Combine(tempPath, "Logs", "LAS_api", fileName);
                AddWriteToAsync(loggerSink, path, FileOutputTemplate);
            }
        }

        private void AddWriteToAsync(LoggerSinkConfiguration loggerSink, string path, string outputTemplate)
        {
            _logFiles.Add(path);

            loggerSink.Async(a => a.File(
                path,
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: 10 * 1024 * 1024,
                retainedFileCountLimit: 2,
                rollOnFileSizeLimit: true,
                shared: true,
                flushToDiskInterval: TimeSpan.FromSeconds(1),
                outputTemplate: outputTemplate));
        }
    }
}