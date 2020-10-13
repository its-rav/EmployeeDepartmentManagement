using AutoMapper;
using BusinessTier.Requests.UserRequest;
using BusinessTier.ViewModels;
using DataTier.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeDepartmentManagement.App_Start
{
    public class AutoMapperConfig
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.CreateMap<CreateAccountRequest, Account>();
                mc.CreateMap<Account,UserViewModel>()
                .ForMember(dest=>dest.RoleName,opt=>opt.MapFrom(src=>src.Role.RoleName))
                .ForMember(dest => dest.LastUpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy))
                .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.Departments, opt => opt.MapFrom((src,dest,member,ctx) => {
                    var departments=src.DepartmentStaff.Select(x => x.Department).ToList();
                    return ctx.Mapper.Map<List<DepartmentViewModel>>(departments);
                }));
                mc.CreateMap<Account, StaffViewModel>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName))
                .ForMember(dest => dest.LastUpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy))
                .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.Departments, opt => opt.MapFrom((src, dest, member, ctx) => {
                    var departments = src.DepartmentStaff.Select(x => x.Department).ToList();
                    return ctx.Mapper.Map<List<DepartmentViewModel>>(departments);
                }));
                mc.CreateMap<Department, DepartmentViewModel>();
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}
