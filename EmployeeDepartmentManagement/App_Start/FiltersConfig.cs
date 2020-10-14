using BusinessTier.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeDepartmentManagement.App_Start
{
    public class FilterConfig
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(ops =>
            {
                ops.ValueProviderFactories.Add(new SnakeCaseQueryValueProviderFactory());
            });
            services.AddControllers(options =>
            {
                options.Filters.Add<ErrorHandlingFilter>();
            });
        }
    }
}
