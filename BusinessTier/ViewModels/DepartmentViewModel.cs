using System;
using System.Collections.Generic;

namespace BusinessTier.ViewModels
{
    public class DepartmentViewModel
    {
        public DepartmentViewModel()
        {
            Staffs = new HashSet<StaffViewModel>();
        }

        public string Id { get; set; }
        public string DepartmentName { get; set; }
        public string Hotline { get; set; }
        public string RoomNumber { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public virtual ICollection<StaffViewModel> Staffs { get; set; }
    }
}
