using AutoMapper;
using BusinessTier.Utilities;
using BusinessTier.ViewModels;
using DataTier.Models;
using DataTier.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BusinessTier.Services
{
    public interface IStaffService
    {
        List<StaffViewModel> GetStaffs();
    }
    public class StaffService : IStaffService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StaffService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper.ConfigurationProvider.CreateMapper();
        }
        public List<StaffViewModel> GetStaffs()
        {
            var staffs = _unitOfWork.Repository<Account>()
                .FindAllByProperty(x => x.Role.Id == Constants.ROLE_STAFF_ID && !x.IsDeleted)
                .Include(x => x.Role)
                .Include(x => x.DepartmentStaff).ThenInclude(x => x.Department)
                .ToList();
            var result = _mapper.Map<List<StaffViewModel>>(staffs);


            return result;
        }
    }
}
