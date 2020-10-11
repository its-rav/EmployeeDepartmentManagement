using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
namespace EmployeeDepartmentManagement.App_Start
{
    public class JsonFormatConfig
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                //options.SerializerSettings.Converters.Add(new StringEnumConverter
                //{
                //    NamingStrategy = new CamelCaseNamingStrategy(),
                //});
                // Use the default property (Pascal) casing


                options.SerializerSettings.ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() };
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Populate;
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
#if DEBUG
                options.SerializerSettings.Formatting = Formatting.Indented;
#else
                options.SerializerSettings.Formatting = Formatting.None;
#endif
            });
        }
    }
}
