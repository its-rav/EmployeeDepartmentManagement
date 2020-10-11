﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeDepartmentManagement.App_Start
{
    public class CorsConfig
    {
        private static string _AllowSpecificOrigins = "_myAllowSpecificOrigins";
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(_AllowSpecificOrigins,
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });
        }
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(_AllowSpecificOrigins);
        }
    }
}
