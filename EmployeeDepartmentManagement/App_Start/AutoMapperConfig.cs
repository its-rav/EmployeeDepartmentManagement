using AutoMapper;
using BusinessTier.Requests.DepartmentRequest;
using BusinessTier.Requests.StaffRequest;
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
                mc.CreateMap<CreateAccountRequest, Account>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()); ;
                mc.CreateMap<UpdateStaffRequest, Account>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()); ;
                mc.CreateMap<CreateStaffRequest, Account>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()); ;
                mc.CreateMap<Account, UserViewModel>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName))
                .ForMember(dest => dest.LastUpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy))
                .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.Departments, opt => opt.MapFrom((src, dest, member, ctx) =>
                {
                    var departments = src.DepartmentStaff.Select(x => x.Department).ToList();
                    return ctx.Mapper.Map<List<DepartmentViewModel>>(departments);
                }));
                mc.CreateMap<Account, StaffViewModel>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName))
                .ForMember(dest => dest.LastUpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy))
                .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.Departments, opt => opt.MapFrom((src, dest, member, ctx) =>
                {
                    var departments = src.DepartmentStaff.Select(x => x.Department).ToList();
                    return ctx.Mapper.Map<List<DepartmentViewModel>>(departments);
                }));
                mc.CreateMap<UpdateDepartmentRequest, Department>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
                mc.CreateMap<CreateDepartmentRequest, Department>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
                mc.CreateMap<Department, DepartmentViewModel>()
                .ForMember(dest => dest.LastUpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy))
                .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.Staffs, opt => opt.MapFrom((src, dest, member, ctx) =>
                {
                    var staffs = src.DepartmentStaff.Select(x => x.Account).ToList();
                    return ctx.Mapper.Map<List<StaffViewModel>>(staffs);
                })); ;
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}
