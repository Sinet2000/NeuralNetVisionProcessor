using System.Reflection;
using NetVisionProc.Application;

namespace NetVisionProc.Api.Setup
{
    public class SwaggerSetup
    {
        private readonly IHostEnvironment _env;

        public SwaggerSetup(IHostEnvironment env)
        {
            _env = env;
        }

        public void Configure(IServiceCollection services)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(opt =>
            {
                opt.EnableAnnotations();
                opt.DescribeAllParametersInCamelCase();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var servicesXmlFile = Path.Combine(
                    AppContext.BaseDirectory,
                    $"{typeof(HealthCheckTestService).Assembly.GetName().Name}.xml");
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                opt.IncludeXmlComments(xmlPath);
                opt.IncludeXmlComments(servicesXmlFile);
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (!_env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
        }
    }
}