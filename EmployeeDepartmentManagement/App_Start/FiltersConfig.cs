using BusinessTier.Handlers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
