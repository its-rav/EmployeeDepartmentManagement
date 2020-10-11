using DataTier.Models;
using DataTier.UnitOfWork;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Razor.Text;

namespace BusinessTier.Services
{
    public interface IUserService
    {
        Account Authenticate(string username, byte[] password);
        Account FindUserById(string username);
        List<Account> GetUsers();
        Account CreateUser(Account user);

        Account UpdateUser(Account user);
    }
    public class UserService: IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Account Authenticate(string username, byte[] password)
        {
            return _unitOfWork.Repository<Account>().FindFirstByProperty(x=>x.Username.Equals(username)&&x.PasswordHash.SequenceEqual(password)&&x.IsDeleted!=null);

        }
        public Account FindUserById(string username)
        {
            return _unitOfWork.Repository<Account>().FindFirstByProperty(x => x.Username.Equals(username) && x.IsDeleted != null);
        }
        public List<Account> GetUsers()
        {
            return _unitOfWork.Repository<Account>().FindAllByProperty(x => x.IsDeleted != null).ToList();
        }
        public Account CreateUser(Account user)
        {
            var repo = _unitOfWork.Repository<Account>();

            var duplicatedUser = repo.FindFirstByProperty(x => x.Username.Equals(user.Username) && x.IsDeleted != null);

            if(duplicatedUser!= null)
            {
                repo.Insert(user);
                repo.Commit();
            }
            else
            {
                throw new Exception( "Duplicated username");
            }
            return user;
        }
        public Account UpdateUser(Account user)
        {
            var repo = _unitOfWork.Repository<Account>();

            var duplicatedUser = repo.FindFirstByProperty(x => x.Username.Equals(user.Username) && x.IsDeleted != null);

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
