using System;
using System.Collections.Generic;

namespace DataTier.Models
{
    public partial class Account
    {
        public Account()
        {
            DepartmentStaff = new HashSet<DepartmentStaff>();
        }

        public Guid Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Photo { get; set; }
        public int RoleId { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdateBy { get; set; }

        public virtual UserRole Role { get; set; }
        public virtual ICollection<DepartmentStaff> DepartmentStaff { get; set; }
    }
}
