using AutoMapper;
using AutoMapper.Internal;
using BusinessTier.Requests.StaffRequest;
using BusinessTier.Utilities;
using BusinessTier.ViewModels;
using DataTier.Models;
using DataTier.UnitOfWork;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.WebPages;

namespace BusinessTier.Services
{
    public interface IStaffService
    {
        List<StaffViewModel> GetStaffs(int page,int size,IEnumerable<string> roles);
        List<StaffViewModel> SearchStaffsByUsername(string query,int page, int size, IEnumerable<string> roles);
        List<StaffViewModel> GetStaffsOfDepartment(string departmentId, int page, int size, IEnumerable<string> roles); 
        List<StaffViewModel> SearchStaffsOfDepartmentByUserName(string departmentId, string query, int page, int size, IEnumerable<string> roles);
        StaffViewModel GetStaffById(Guid staffId, IEnumerable<string> roles);
        StaffViewModel CreateStaff(CreateStaffRequest request, string createdBy, IEnumerable<string> roles);
        StaffViewModel UpdateStaff(Guid id, UpdateStaffRequest request, string updatedBy, IEnumerable<string> roles);
        string DeleteStaff(Guid id, string updater, IEnumerable<string> roles);
        void AddStaffToDepartment(Guid staffId, string departmentId, string requester, IEnumerable<string> roles);
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
        public List<StaffViewModel> GetStaffs(int page=1, int size=Constants.DEFAULT_PAGE_SIZE,IEnumerable<string> roles = null)
        {
            if (roles.Count() == 0 || roles == null) return null;

            IQueryable<Account> staffs = null;

            //if staff request => get all
            if (roles.Contains(Constants.ROLE_ADMIN_NAME))
            {
                staffs = _unitOfWork.Repository<Account>()
                .FindAllByProperty(x =>true)
                .Include(x => x.Role)
                .Include(x => x.DepartmentStaff).ThenInclude(x => x.Department);
            }
            //if mod request => get only staffs
            else if (roles.Contains(Constants.ROLE_MOD_NAME))
            {
                staffs = _unitOfWork.Repository<Account>()
                .FindAllByProperty(x => x.Role.Id.Equals(Constants.ROLE_STAFF_ID))
                .Include(x => x.Role)
                .Include(x => x.DepartmentStaff).ThenInclude(x => x.Department);
            }

            if (staffs == null) return null;

            var result = staffs.ToPaginatedList<Account>(page, size).IgnoreSecondAccounts();

            return _mapper.Map<List<StaffViewModel>>(result);
        }
        public List<StaffViewModel> SearchStaffsByUsername(string query="", int page=1, int size=Constants.DEFAULT_PAGE_SIZE, IEnumerable<string> roles=null)
        {
            if (size > Constants.MAX_PAGE_SIZE)
                throw new Exception(Constants.ERR_MAX_PAGE_SIZE);

            if (roles.Count() == 0 || roles == null) return null;

            var staffs = (IQueryable<Account>)null;

            //if staff request => get all
            if (roles.Contains(Constants.ROLE_ADMIN_NAME))
            {
                staffs = _unitOfWork.Repository<Account>()
                .FindAllByProperty(x => x.Username.Contains(query.Trim()))
                .Include(x => x.Role)
                .Include(x => x.DepartmentStaff).ThenInclude(x => x.Department);
            }
            //if mod request => get only staffs
            else if (roles.Contains(Constants.ROLE_MOD_NAME))
            {
                staffs = _unitOfWork.Repository<Account>()
                .FindAllByProperty(x => x.Username.Contains(query.Trim()) && x.Role.Id.Equals(Constants.ROLE_STAFF_ID))
                .Include(x => x.Role)
                .Include(x => x.DepartmentStaff).ThenInclude(x => x.Department);
            }

            if (staffs == null) return null;

            var result = staffs.ToPaginatedList<Account>(page, size).IgnoreSecondAccounts();

            return _mapper.Map<List<StaffViewModel>>(result);
        }
        public List<StaffViewModel> GetStaffsOfDepartment(string departmentId, int page = 1, int size = Constants.DEFAULT_PAGE_SIZE, IEnumerable<string> roles = null)
        {
            if (roles.Count() == 0 || roles == null) return null;

            IQueryable<Account> staffs = null;

            if (roles.Contains(Constants.ROLE_ADMIN_NAME))
            {
                staffs = _unitOfWork.Repository<Account>()
                 .Get(x => x.DepartmentStaff.Select(x => x.DepartmentId).Contains(departmentId))
                 .Include(x => x.Role)
                 .Include(x => x.DepartmentStaff).ThenInclude(x => x.Department);
            }
            //if mod request => get only staffs
            else if (roles.Contains(Constants.ROLE_MOD_NAME))
            {
                staffs = _unitOfWork.Repository<Account>()
                .Get(x => x.DepartmentStaff.Select(x => x.DepartmentId).Contains(departmentId)
                                    && x.Role.Id.Equals(Constants.ROLE_STAFF_ID) ) 
                .Include(x => x.Role)
                .Include(x => x.DepartmentStaff).ThenInclude(x => x.Department);
            }

            if (staffs == null) return null;

            var result = staffs.ToPaginatedList<Account>(page, size).IgnoreSecondAccounts();

            return _mapper.Map<List<StaffViewModel>>(result);
        }
        public List<StaffViewModel> SearchStaffsOfDepartmentByUserName(string departmentId,string query="", int page=1, int size = Constants.DEFAULT_PAGE_SIZE, IEnumerable<string> roles = null)
        {
            if (size > Constants.MAX_PAGE_SIZE)
                throw new Exception(Constants.ERR_MAX_PAGE_SIZE);

            if (roles.Count() == 0 || roles == null) return null;

            IQueryable<Account> staffs = null;

            if (roles.Contains(Constants.ROLE_ADMIN_NAME))
            {
                staffs = _unitOfWork.Repository<Account>()
                 .Get(x => x.DepartmentStaff.Select(x => x.DepartmentId).Contains(departmentId) && x.Username.Contains(query.Trim()))
                 .Include(x => x.Role)
                 .Include(x => x.DepartmentStaff).ThenInclude(x => x.Department);
            }
            //if mod request => get only staffs
            else if (roles.Contains(Constants.ROLE_MOD_NAME))
            {
                staffs = _unitOfWork.Repository<Account>()
                .Get(x => x.DepartmentStaff.Select(x => x.DepartmentId).Contains(departmentId)
                                    && x.Role.Id.Equals(Constants.ROLE_STAFF_ID)
                                    && x.Username.Contains(query.Trim()))
                .Include(x => x.Role)
                .Include(x => x.DepartmentStaff).ThenInclude(x => x.Department);
            }

            if (staffs == null) return null;

            var result = staffs.ToPaginatedList<Account>(page, size).IgnoreSecondAccounts();

            return _mapper.Map<List<StaffViewModel>>(result);
        }
        public StaffViewModel GetStaffById(Guid staffId, IEnumerable<string> roles = null)
        {
            if (roles.Count() == 0 || roles == null) return null;

            var staff = new Account();

            if (roles.Contains(Constants.ROLE_ADMIN_NAME))
            {
                staff = _unitOfWork.Repository<Account>()
                .Get(x => x.Id.Equals(staffId))
                .Include(x => x.Role)
                .Include(x => x.DepartmentStaff).ThenInclude(x => x.Department)
                .FirstOrDefault().IgnoreSecondAccounts();
            }
            //if mod request => get only staffs
            else if (roles.Contains(Constants.ROLE_MOD_NAME))
            {
                staff = _unitOfWork.Repository<Account>()
                .Get(x => x.Id.Equals(staffId) && x.Role.Id.Equals(Constants.ROLE_STAFF_ID))
                .Include(x => x.Role)
                .Include(x => x.DepartmentStaff).ThenInclude(x => x.Department)
                .FirstOrDefault().IgnoreSecondAccounts();
            }

            return _mapper.Map<StaffViewModel>(staff);
        }
        public StaffViewModel UpdateStaff(Guid id, UpdateStaffRequest request, string updatedBy, IEnumerable<string> roles = null)
        {
            if (roles.Count() == 0 || roles == null) return null;

            if (!request.Password.IsEmpty() && request.PasswordConfirmation.IsEmpty())
                throw new Exception(Constants.ERR_EMPTY_PWD_CONFIR);
            else if (!request.Password.IsEmpty() && !request.Password.Equals(request.PasswordConfirmation))
                throw new Exception(Constants.ERR_PWD_NOTMATCH);

            if (request.Email.IsEmpty())
                throw new Exception(Constants.ERR_EMPTY_EMAIL);
            if (request.FullName.IsEmpty())
                throw new Exception(Constants.ERR_EMPTY_FNAME);
            if (request.IsDeleted==null)
                throw new Exception(Constants.ERR_EMPTY_DELFLAG);
            if (request.PhoneNumber.Length > Constants.CONSTRAINT_PHONENUM_MAXLEN)
                throw new Exception(Constants.ERR_PHONENUM_MAXLEN);

            var repo = _unitOfWork.Repository<Account>();

            var updatingStaff = _mapper.Map<Account>(request);
            var staff = new Account();
            try
            {
                staff = repo.Get(x => x.Id.Equals(id))
                    .Include(x=>x.Role).FirstOrDefault();

                //not found
                if (staff == null)
                    return null;
                //cant not modify mod or admin if youre not admin
                else if ((staff.Role.RoleName.Equals(Constants.ROLE_MOD_NAME) || staff.Role.RoleName.Equals(Constants.ROLE_ADMIN_NAME) )
                    && !roles.Contains(Constants.ROLE_ADMIN_NAME))
                    return null;

                //proceed to update

                var type = typeof(Account);
                var props = type.GetProperties().Where(x => x.CanWrite && x.CanRead);

                props.ForAll(x =>
                {
                    if (x.GetValue(updatingStaff) != null 
                    && ! new[] { nameof(Account.UpdatedAt), nameof(Account.RoleId), nameof(Account.Id), nameof(Account.CreatedAt) }.Contains(x.Name))
                        x.SetValue(staff, x.GetValue(updatingStaff));
                });

                if (!updatingStaff.RoleId.Equals(0))
                    staff.RoleId = updatingStaff.RoleId;

                if(!request.Password.IsEmpty())
                    staff.PasswordHash = IdentityManager.HashPassword(request.Password);
                staff.UpdatedBy = updatedBy;
                staff.UpdatedAt = DateTime.Now;
                repo.Update(staff);
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



            return _mapper.Map<StaffViewModel>(staff);
        }
        public StaffViewModel CreateStaff(CreateStaffRequest request, string createdBy, IEnumerable<string> roles = null)
        {
            if (roles.Count() == 0 || roles == null) return null;

            if (roles.Contains(Constants.ROLE_MOD_NAME) && !request.RoleId.Equals(Constants.ROLE_STAFF_ID))
                return null;

            if (!request.Password.IsEmpty() && request.PasswordConfirmation.IsEmpty())
                throw new Exception(Constants.ERR_EMPTY_PWD_CONFIR);
            else if (!request.Password.IsEmpty() && !request.Password.Equals(request.PasswordConfirmation))
                throw new Exception(Constants.ERR_PWD_NOTMATCH);

            if (request.Email.IsEmpty())
                throw new Exception(Constants.ERR_EMPTY_EMAIL);
            if (request.FullName.IsEmpty())
                throw new Exception(Constants.ERR_EMPTY_FNAME);
            if (request.PhoneNumber.Length > Constants.CONSTRAINT_PHONENUM_MAXLEN)
                throw new Exception(Constants.ERR_PHONENUM_MAXLEN);


            var repo = _unitOfWork.Repository<Account>();

            var creatingStaff = _mapper.Map<Account>(request);

            try
            {
                creatingStaff.Id = Guid.NewGuid();
                creatingStaff.CreatedBy = createdBy;
                creatingStaff.UpdatedBy = createdBy;
                creatingStaff.PasswordHash = IdentityManager.HashPassword(request.Password);
                repo.Insert(creatingStaff);
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



            return _mapper.Map<StaffViewModel>(creatingStaff);
        }
        public string DeleteStaff(Guid id, string updater, IEnumerable<string> roles=null)
        {
            if (roles.Count() == 0 || roles == null) return null;

            var repo = _unitOfWork.Repository<Account>();

            var staff = repo.FindAllByProperty(x=>x.Id.Equals(id) && !x.IsDeleted)
                .Include(x=>x.Role).FirstOrDefault();

            if (staff != null)
            {
                if (staff.Role.RoleName.Equals(Constants.ROLE_MOD_NAME) || staff.Role.RoleName.Equals(Constants.ROLE_ADMIN_NAME)
                    && !roles.Contains(Constants.ROLE_ADMIN_NAME)) return null;

                staff.IsDeleted = true;
                staff.UpdatedAt = DateTime.Now;
                staff.UpdatedBy = updater;
                var staffId = staff.Id.ToString();

                repo.Update(staff);
                repo.Commit();

                return staffId;
            }
            

            return null;
        }
        public void AddStaffToDepartment( Guid staffId,string departmentId, string requester, IEnumerable<string> roles = null)
        {

            if (roles.Count() == 0 || roles == null) return;

            var dsRepo = _unitOfWork.Repository<DepartmentStaff>();
            var aRepo = _unitOfWork.Repository<Account>();
            try
            {
                Account staff = aRepo.Get(x => x.Id.Equals(staffId)).Include(x => x.Role).FirstOrDefault();

                if (staff == null)
                    throw new Exception(Constants.ERR_STAFF_NOTFOUND);

                if (roles.Contains(Constants.ROLE_MOD_NAME)
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

    static class StaffServiceExtensions
    {
        public static List<Account> IgnoreSecondAccounts(this List<Account> source)
        {
            if (source == null) return null;
            foreach (var account in source)
                foreach (var ds in account.DepartmentStaff)
                    ds.Department.DepartmentStaff = new HashSet<DepartmentStaff>();
            return source;
        }
        public static Account IgnoreSecondAccounts(this Account source)
        {
            if (source == null) return null;
            foreach (var ds in source.DepartmentStaff)
                ds.Department.DepartmentStaff = new HashSet<DepartmentStaff>();
            return source;
        }
    }
}
