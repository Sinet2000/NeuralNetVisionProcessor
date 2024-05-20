using NuGet.Packaging;

namespace ClientApp.Setup
{
    public class CorsSetup
    {
        private readonly HashSet<string> _allowedOrigins = new(StringComparer.OrdinalIgnoreCase);

        public CorsSetup(IConfiguration config)
        {
            var apiConfig = config.GetSection(Config.ApiConfig.SectionName).Get<Config.ApiConfig>();

            Guard.Against.Null(apiConfig, nameof(apiConfig));
            Guard.Against.NullOrEmpty(apiConfig.AllowedOrigins, nameof(apiConfig.AllowedOrigins));

            _allowedOrigins.AddRange(apiConfig.AllowedOrigins);
        }

        public void Configure(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(p =>
                    p.SetIsOriginAllowed(IsOriginAllowed)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithExposedHeaders("Content-Disposition"));
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCors();
        }

        private bool IsOriginAllowed(string origin)
        {
            var uri = new Uri(origin);
            return _allowedOrigins.Contains(uri.Host);
        }
    }
}