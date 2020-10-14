using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace EmployeeDepartmentManagement.App_Start
{
    public class ErrorHandlerConfig
    {
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseMiddleware<ErrorHandlingFilter>();
                //app.UseExceptionHandler("/error");
                //app.UseHsts();
            }
            else
            {
                //app.UseMiddleware<ErrorHandlingFilter>();
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

        }
    }
}
