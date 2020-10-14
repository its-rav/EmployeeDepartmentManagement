using AutoMapper;
using BusinessTier.Requests.UserRequest;
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
    public interface IUserService
    {
        (string, UserViewModel) Authenticate(string username, string password);
        Account FindUserById(string username);
        List<UserViewModel> GetUsers();
        UserViewModel CreateUser(CreateAccountRequest request, string createdBy);

        Account UpdateUser(Account user);
    }
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper.ConfigurationProvider.CreateMapper();
        }
        public (string, UserViewModel) Authenticate(string username, string password)
        {
            var user = _unitOfWork.Repository<Account>().FindFirstByProperty(x => x.Username.Equals(username) && IdentityManager.VerifyHashedPassword(x.PasswordHash, password) && !x.IsDeleted);
            if (user == null) return (null, null);
            return (IdentityManager.GenerateJwtToken(user.FullName, new string[] { user.Role.RoleName }, user.Id, user.Username)
                , _mapper.Map<UserViewModel>(user));

        }
        public Account FindUserById(string username)
        {
            return _unitOfWork.Repository<Account>().FindFirstByProperty(x => x.Username.Equals(username) && !x.IsDeleted);
        }
        public List<UserViewModel> GetUsers()
        {
            var users = _unitOfWork.Repository<Account>()
                .FindAllByProperty(x => !x.IsDeleted)
                .Include(x => x.Role)
                //.Include(x=>x.DepartmentStaff).ThenInclude(x=>x.Department)
                .ToList();
            var result = _mapper.Map<List<UserViewModel>>(users);


            return result;
        }
        public UserViewModel CreateUser(CreateAccountRequest request, string createdBy)
        {
            if (request.Password.IsEmpty() || request.PasswordConfirmation.IsEmpty())
                throw new Exception(Constants.ERR_EMPTY_PWD);
            if (request.Email.IsEmpty())
                throw new Exception(Constants.ERR_EMPTY_EMAIL);
            if (request.FullName.IsEmpty())
                throw new Exception(Constants.ERR_EMPTY_FNAME);
            if (!request.Password.Equals(request.PasswordConfirmation))
                throw new Exception(Constants.ERR_PWD_NOTMATCH);


            var repo = _unitOfWork.Repository<Account>();

            var creatingUser = _mapper.Map<Account>(request);

            try
            {
                creatingUser.Id = Guid.NewGuid();
                creatingUser.CreatedBy = createdBy;
                creatingUser.UpdatedBy = createdBy;
                creatingUser.PasswordHash = IdentityManager.HashPassword(request.Password);
                repo.Insert(creatingUser);
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



            return _mapper.Map<UserViewModel>(creatingUser);
        }
        public Account UpdateUser(Account user)
        {
            var repo = _unitOfWork.Repository<Account>();

            var duplicatedUser = repo.FindFirstByProperty(x => x.Username.Equals(user.Username) && !x.IsDeleted);

            if (duplicatedUser != null)
            {
                repo.Update(user);
                repo.Commit();
            }
            else
            {
                return null;
            }

            return user;
        }

    }
}
