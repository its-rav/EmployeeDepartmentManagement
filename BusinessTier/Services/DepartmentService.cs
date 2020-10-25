using AutoMapper;
using AutoMapper.Internal;
using BusinessTier.Requests.DepartmentRequest;
using BusinessTier.Utilities;
using BusinessTier.ViewModels;
using DataTier.Models;
using DataTier.UnitOfWork;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.WebPages;

namespace BusinessTier.Services
{
    public interface IDepartmentService
    {
        List<DepartmentViewModel> GetDepartments(int page,int size,IEnumerable<string> roles);
        List<DepartmentViewModel> SearchDepartmentsByName(string query,int page, int size, IEnumerable<string> roles);
        List<DepartmentViewModel> GetDepartmentsOfStaff(Guid staffId, int page, int size, IEnumerable<string> roles);
        List<DepartmentViewModel> SearchDepartmentsOfStaffByName(Guid staffId, string query, int page, int size, IEnumerable<string> roles);
        DepartmentViewModel GetDepartmentById(string departmentId, IEnumerable<string> roles);
        DepartmentViewModel CreateDepartment(CreateDepartmentRequest request, string createdBy, IEnumerable<string> roles);
        DepartmentViewModel UpdateDepartment(string id, UpdateDepartmentRequest request, string updatedBy, IEnumerable<string> roles);
        string DeleteDepartment(string id, string updater, IEnumerable<string> roles);
        void AddStaffToDepartment(string departmentId,Guid staffId, string requester, IEnumerable<string> roles);
    }
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public DepartmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper.ConfigurationProvider.CreateMapper();
        }
        public DepartmentViewModel CreateDepartment(CreateDepartmentRequest request, string createdBy, IEnumerable<string> roles = null)
        {
            if (roles.Count() == 0 || roles == null) return null;

            if (request.Id.IsEmpty())
                throw new Exception(Constants.ERR_EMPTY_ID);
            else if (request.Id.Length > Constants.CONSTRAINT_DEPID_MAXLEN)
                throw new Exception(Constants.ERR_DEPID_MAXLEN);

            if (request.RoomNumber.IsEmpty())
                throw new Exception(Constants.ERR_EMPTY_ROOMNUM);
            else if (request.RoomNumber.Length > Constants.CONSTRAINT_ROOMNUM_MAXLEN)
                throw new Exception(Constants.ERR_ROOMNUM_MAXLEN);

            if (request.Hotline.Length > Constants.CONSTRAINT_HOTLINE_MAXLEN)
                throw new Exception(Constants.ERR_HOTLINE_MAXLEN);

            var repo = _unitOfWork.Repository<Department>();

            var creatingDepartment = _mapper.Map<Department>(request);

            try
            {
                creatingDepartment.CreatedBy = createdBy;
                creatingDepartment.UpdatedBy = createdBy;
                repo.Insert(creatingDepartment);
                repo.Commit();
            }
            catch (DbUpdateException due)
            {
                var se = due.GetBaseException() as SqlException;
                if (se != null)
                {
                    if (se.Errors.Count > 0)
                    {
                        switch (se.Errors[0].Number)
                        {
                            case 547: // Foreign Key violation
                                throw new Exception(Constants.ERR_ROLE_FK);
                            case 2627:
                                throw new Exception(Constants.ERR_PK_EXIST);
                            default:
                                throw;
                        }
                    }
                }
            }



            return _mapper.Map<DepartmentViewModel>(creatingDepartment);
        }

        public string DeleteDepartment(string id, string updater, IEnumerable<string> roles = null)
        {
            if (roles.Count() == 0 || roles == null) return null;

            var repo = _unitOfWork.Repository<Department>();

            var department = repo.FindAllByProperty(x => x.Id.Equals(id) && !x.IsDeleted)
                .Include(x => x.DepartmentStaff)
                .ThenInclude(x=>x.Account).ThenInclude(x=>x.Role)
                .FirstOrDefault();

            if (department != null)
            {
                department.IsDeleted = true;
                department.UpdatedAt = DateTime.Now;
                department.UpdatedBy = updater;
                var departmentId = department.Id.ToString();

                repo.Update(department);
                repo.Commit();

                return departmentId;
            }


            return null;
        }

        public DepartmentViewModel GetDepartmentById(string departmentId, IEnumerable<string> roles = null)
        {
            if (roles.Count() == 0 || roles == null) return null;

            var department = new Department();

            //if staff request => get all
            if (roles.Contains(Constants.ROLE_ADMIN_NAME))
            {
                department = _unitOfWork.Repository<Department>()
                .FindAllByProperty(x => x.Id.Equals(departmentId))
                .Include(x => x.DepartmentStaff)
                .ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                .FirstOrDefault().IgnoreSecondDepartments();
            }
            //if mod request => get only staffs
            else if (roles.Contains(Constants.ROLE_MOD_NAME))
            {
                department = _unitOfWork.Repository<Department>()
                .FindAllByProperty(x => x.Id.Equals(departmentId))
                .Include(x => x.DepartmentStaff)
                .ThenInclude(x => x.Account).ThenInclude(x => x.Role)
                .FirstOrDefault().FilterForModRole();
            }

            return _mapper.Map<DepartmentViewModel>(department);
        }

        public List<DepartmentViewModel> GetDepartments(int page=1,int size=Constants.DEFAULT_PAGE_SIZE,IEnumerable<string> roles = null)
        {
            if (size > Constants.MAX_PAGE_SIZE)
                throw new Exception(Constants.ERR_MAX_PAGE_SIZE);

            if (roles.Count() == 0 || roles == null) return null;

             List<Department> result= null;

            //if staff request => get all
            if (roles.Contains(Constants.ROLE_ADMIN_NAME))
            {
                IQueryable<Department> departments = _unitOfWork.Repository<Department>()
                .FindAllByProperty(x => true)
                .Include(x => x.DepartmentStaff)
                .ThenInclude(x => x.Account).ThenInclude(x => x.Role);

                result = departments.ToPaginatedList<Department>(page, size).ToList().IgnoreSecondDepartments();
            }
            //if mod request => get only staffs
            else if (roles.Contains(Constants.ROLE_MOD_NAME))
            {
                IQueryable<Department> departments = _unitOfWork.Repository<Department>()
                .FindAllByProperty(x => true)
                .Include(x => x.DepartmentStaff)
                .ThenInclude(x => x.Account).ThenInclude(x => x.Role);

                result = departments.ToPaginatedList<Department>(page, size).ToList().FilterForModRole();
            }

            if (result == null) return null;

            return _mapper.Map<List<DepartmentViewModel>>(result);
        }
        public List<DepartmentViewModel> SearchDepartmentsByName(string query="",int page=1,int size=Constants.DEFAULT_PAGE_SIZE,IEnumerable<string> roles = null)
        {
            if (size > Constants.MAX_PAGE_SIZE)
                throw new Exception(Constants.ERR_MAX_PAGE_SIZE);

            if (roles.Count() == 0 || roles == null) return null;

            List<Department> result = null;

            //if staff request => get all
            if (roles.Contains(Constants.ROLE_ADMIN_NAME))
            {
                IQueryable<Department> departments = _unitOfWork.Repository<Department>()
                .FindAllByProperty(x => x.DepartmentName.Contains(query.Trim()))
                .Include(x => x.DepartmentStaff)
                .ThenInclude(x => x.Account).ThenInclude(x => x.Role);

                result = departments.ToPaginatedList<Department>(page, size).ToList().IgnoreSecondDepartments();
            }
            //if mod request => get only staffs
            else if (roles.Contains(Constants.ROLE_MOD_NAME))
            {
                IQueryable<Department> departments = _unitOfWork.Repository<Department>()
                .FindAllByProperty(x => x.DepartmentName.Contains(query.Trim()))
                .Include(x => x.DepartmentStaff)
                .ThenInclude(x => x.Account).ThenInclude(x => x.Role);

                result = departments.ToPaginatedList<Department>(page, size).ToList().FilterForModRole();
            }

            if (result == null) return null;

            return _mapper.Map<List<DepartmentViewModel>>(result);
        }
        public List<DepartmentViewModel> GetDepartmentsOfStaff(Guid staffId,int page=1,int size=Constants.DEFAULT_PAGE_SIZE, IEnumerable<string> roles = null)
        {
            if (size > Constants.MAX_PAGE_SIZE)
                throw new Exception(Constants.ERR_MAX_PAGE_SIZE);

            if (roles.Count() == 0 || roles == null) return null;

            List<Department> result = null;

            if (roles.Contains(Constants.ROLE_ADMIN_NAME))
            {
                IQueryable<Department> departments = _unitOfWork.Repository<Department>()
                 .Get(x => x.DepartmentStaff.Select(x => x.AccountId).Contains(staffId))
                 .Include(x => x.DepartmentStaff).ThenInclude(x => x.Account).ThenInclude(x => x.Role);

                 result=departments.ToPaginatedList<Department>(page,size).IgnoreSecondDepartments();
            }
            //if mod request => get only staffs
            else if (roles.Contains(Constants.ROLE_MOD_NAME))
            {

                var staff = _unitOfWork.Repository<Account>().Get(x => x.Id.Equals(staffId)).FirstOrDefault();
                if (!staff.RoleId.Equals(Constants.ROLE_STAFF_ID))
                    return null;

                IQueryable<Department> departments = _unitOfWork.Repository<Department>()
                .Get(x => x.DepartmentStaff.Select(x => x.AccountId).Contains(staffId))
                .Include(x => x.DepartmentStaff)
                .ThenInclude(x => x.Account).ThenInclude(x => x.Role);
                result = departments.ToPaginatedList<Department>(page, size).FilterForModRole();
            }

            if (result == null) return null;

            return _mapper.Map<List<DepartmentViewModel>>(result);
        }
        public List<DepartmentViewModel> SearchDepartmentsOfStaffByName(Guid staffId,string query, int page = 1, int size = Constants.DEFAULT_PAGE_SIZE, IEnumerable<string> roles = null)
        {
            if (size > Constants.MAX_PAGE_SIZE)
                throw new Exception(Constants.ERR_MAX_PAGE_SIZE);

            if (roles.Count() == 0 || roles == null) return null;

            List<Department> result = null;

            if (roles.Contains(Constants.ROLE_ADMIN_NAME))
            {
                IQueryable<Department> departments = _unitOfWork.Repository<Department>()
                 .Get(x => x.DepartmentStaff.Select(x => x.AccountId).Contains(staffId))
                 .Include(x => x.DepartmentStaff).ThenInclude(x => x.Account).ThenInclude(x => x.Role);

                 result=departments.ToPaginatedList<Department>(page,size).IgnoreSecondDepartments();
            }
            //if mod request => get only staffs
            else if (roles.Contains(Constants.ROLE_MOD_NAME))
            {
                var staff = _unitOfWork.Repository<Account>().Get(x => x.Id.Equals(staffId)).FirstOrDefault();
                if (!staff.RoleId.Equals(Constants.ROLE_STAFF_ID))
                    return null;

                IQueryable<Department> departments = _unitOfWork.Repository<Department>()
                .Get(x => x.DepartmentStaff.Select(x => x.AccountId).Contains(staffId))
                .Include(x => x.DepartmentStaff)
                .ThenInclude(x => x.Account).ThenInclude(x => x.Role);
                result = departments.ToPaginatedList<Department>(page, size).FilterForModRole();
            }

            if (result == null) return null;

            return _mapper.Map<List<DepartmentViewModel>>(result);
        }
        public DepartmentViewModel UpdateDepartment(string id, UpdateDepartmentRequest request, string updatedBy, IEnumerable<string> roles = null)
        {

            if (roles.Count() == 0 || roles == null) return null;

            if (id.IsEmpty())
                throw new Exception(Constants.ERR_EMPTY_ID);
            if (request.DepartmentName.IsEmpty())
                throw new Exception(Constants.ERR_EMPTY_DNAME);
            if (request.IsDeleted == null)
                throw new Exception(Constants.ERR_EMPTY_DELFLAG);

            if (request.RoomNumber.IsEmpty())
                throw new Exception(Constants.ERR_EMPTY_ROOMNUM);
            else if (request.RoomNumber.Length > Constants.CONSTRAINT_ROOMNUM_MAXLEN)
                throw new Exception(Constants.ERR_ROOMNUM_MAXLEN);

            if (request.Hotline.Length > Constants.CONSTRAINT_HOTLINE_MAXLEN)
                throw new Exception(Constants.ERR_HOTLINE_MAXLEN);

            var repo = _unitOfWork.Repository<Department>();

            var updatingDepartment = _mapper.Map<Department>(request);

            var department = new Department();
            try
            {
                department = repo.Get(x => x.Id.Equals(id)).FirstOrDefault();

                //not found
                if (department == null)
                    return null;

                //proceed to update

                if(updatingDepartment.RoomNumber!=null)
                    department.RoomNumber = updatingDepartment.RoomNumber;
                if (updatingDepartment.DepartmentName != null)
                    department.DepartmentName = updatingDepartment.DepartmentName;
                if (updatingDepartment.Hotline != null)
                    department.Hotline = updatingDepartment.Hotline;
                if (updatingDepartment.IsDeleted != null)
                    department.IsDeleted = updatingDepartment.IsDeleted;

                department.UpdatedBy = updatedBy;
                department.UpdatedAt = DateTime.Now;
                repo.Update(department);
                repo.Commit();
            }
            catch (DbUpdateException due)
            {
                var se = due.GetBaseException() as SqlException;
                if (se != null)
                {
                    if (se.Errors.Count > 0)
                    {
                        switch (se.Errors[0].Number)
                        {
                            case 547: // Foreign Key violation
                                throw new Exception(Constants.ERR_ROLE_FK);
                            case 2627:
                                throw new Exception(Constants.ERR_UNAME_NOTAVAILABLE);
                            default:
                                throw;
                        }
                    }
                }
            }

            return _mapper.Map<DepartmentViewModel>(department);
        }

        public void AddStaffToDepartment(string departmentId, Guid staffId, string requester, IEnumerable<string> roles = null)
        {

            if (roles.Count() == 0 || roles == null) return;

            var dsRepo = _unitOfWork.Repository<DepartmentStaff>();
            var aRepo = _unitOfWork.Repository<Account>();
            try
            {
                Account staff = aRepo.Get(x => x.Id.Equals(staffId)).Include(x => x.Role).FirstOrDefault();

                if (staff == null)
                    throw new Exception(Constants.ERR_STAFF_NOTFOUND);

                if(roles.Contains(Constants.ROLE_MOD_NAME) 
                    && staff.Role.RoleName.Equals(Constants.ROLE_MOD_NAME)
                    || staff.Role.RoleName.Equals(Constants.ROLE_ADMIN_NAME))
                    throw new Exception(Constants.ERR_MOD_PERM);


                dsRepo.Insert(new DepartmentStaff()
                {
                    AccountId = staffId,
                    DepartmentId = departmentId,
                    CreatedBy = requester,
                    UpdatedBy = requester
                });
                dsRepo.Commit();
            }
            catch (DbUpdateException due)
            {
                var se = due.GetBaseException() as SqlException;
                if (se != null)
                {
                    if (se.Errors.Count > 0)
                    {
                        switch (se.Errors[0].Number)
                        {
                            case 547: // Foreign Key violation
                                throw new Exception(Constants.ERR_FK);
                            default:
                                throw;
                        }
                    }
                }
            }
        }
    }
    static class DepartmentServiceExtensions
    {
        public static List<Department> IgnoreSecondDepartments(this List<Department> source)
        {
            if (source == null) return null;
            foreach (var department in source)
                foreach (var ds in department.DepartmentStaff)
                    ds.Account.DepartmentStaff = new HashSet<DepartmentStaff>();
            return source;
        }
        public static Department IgnoreSecondDepartments(this Department source)
        {
            if (source == null) return null;
            foreach (var ds in source.DepartmentStaff)
                ds.Account.DepartmentStaff = new HashSet<DepartmentStaff>();
            return source;
        }
        public static List<Department> FilterForModRole(this List<Department> source)
        {
            if (source == null) return null;
            foreach (var department in source)
            {
                List<DepartmentStaff> newDsList = new List<DepartmentStaff>();
                foreach (var ds in department.DepartmentStaff)
                {
                    ds.Account.DepartmentStaff = new HashSet<DepartmentStaff>();
                    if (ds.Account.Role.Id.Equals(Constants.ROLE_STAFF_ID))
                    {
                        newDsList.Add(ds);
                    }
                }
                department.DepartmentStaff = newDsList;
            }
                
            return source;
        }
        public static Department FilterForModRole(this Department source)
        {
            if (source == null) return null;
            List<DepartmentStaff> newDsList = new List<DepartmentStaff>();
            foreach (var ds in source.DepartmentStaff)
            {
                ds.Account.DepartmentStaff = new HashSet<DepartmentStaff>();
                if (ds.Account.Role.Id.Equals(Constants.ROLE_STAFF_ID))
                {
                    newDsList.Add(ds);
                }
            }
            source.DepartmentStaff = newDsList;

            return source;
        }
    }
}
