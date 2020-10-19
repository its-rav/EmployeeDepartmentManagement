using BusinessTier.Services;
using DataTier.Models;
using DataTier.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeDepartmentManagement.App_Start
{
    public class DependencyInjectionResolver
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<DbContext, EDMContext>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
        }
    }
}
