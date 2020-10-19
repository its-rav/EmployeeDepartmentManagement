using System;
using System.Collections.Generic;

namespace BusinessTier.ViewModels
{
    public class StaffViewModel
    {
        public StaffViewModel()
        {
            Departments = new HashSet<DepartmentViewModel>();
        }
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Photo { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public virtual ICollection<DepartmentViewModel> Departments { get; set; }
    }
}
