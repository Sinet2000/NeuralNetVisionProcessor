using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using NetVisionProc.Api.Infra.Filters;
using NetVisionProc.Common.Serialization;

namespace NetVisionProc.Api.Setup
{
    [ExcludeFromCodeCoverage]
    public class ApiSetup
    {
        private readonly SwaggerSetup _swaggerSetup;

        public ApiSetup(IHostEnvironment env)
        {
            _swaggerSetup = new SwaggerSetup(env);
        }

        public void Configure(IServiceCollection services)
        {
            services.Configure<RouteOptions>(opt =>
            {
                opt.LowercaseUrls = true;
                opt.LowercaseQueryStrings = true;
            });

            services.AddControllers(opt =>
                {
                    opt.Filters.Add(new ResponseCacheAttribute
                    {
                        NoStore = true,
                        Location = ResponseCacheLocation.None
                    });

                    opt.Filters.Add(typeof(ApiGlobalExceptionFilter));

                    opt.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
                })
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.WriteIndented = true;

                    opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

                    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    opt.JsonSerializerOptions.Converters.Add(new JsonStringTrimConverter());

                    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });

            _swaggerSetup.Configure(services);
        }

        public void Configure(WebApplication app)
        {
            app.MapControllers();
            _swaggerSetup.Configure(app);
        }
    }
}