using System;
using System.Collections.Generic;

namespace BusinessTier.ViewModels
{
    public class DepartmentViewModel
    {
        public DepartmentViewModel()
        {
            Staffs = new HashSet<UserViewModel>();
        }

        public string Id { get; set; }
        public string DepartmentName { get; set; }
        public string Hotline { get; set; }
        public string RoomNumber { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdateBy { get; set; }

        public virtual ICollection<UserViewModel> Staffs { get; set; }
    }
}
