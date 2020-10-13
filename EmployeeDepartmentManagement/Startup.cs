using EmployeeDepartmentManagement.App_Start;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace EmployeeDepartmentManagement
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            FilterConfig.ConfigureServices(services);

            DependencyInjectionResolver.ConfigureServices(services);

            JsonFormatConfig.ConfigureServices(services);

            SwaggerConfig.ConfigureServices(services);

            AutoMapperConfig.ConfigureServices(services);

            AuthConfig.ConfigureServices(services);

            DbConfig.ConfigureServices(services, Configuration);

            //sql connection
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddControllers();

            CorsConfig.ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {

            ErrorHandlerConfig.Configure(app, env);

            app.UseHttpsRedirection();

            app.UseRouting();

            AuthConfig.Configure(app, env);

            CorsConfig.Configure(app, env);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            SwaggerConfig.Configure(app, env, provider);
        }
    }
}
