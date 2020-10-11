using System;
using System.Collections.Generic;

namespace DataTier.Models
{
    public partial class Department
    {
        public Department()
        {
            DepartmentStaff = new HashSet<DepartmentStaff>();
        }

        public string Id { get; set; }
        public string DepartmentName { get; set; }
        public string Hotline { get; set; }
        public string RoomNumber { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdateBy { get; set; }

        public virtual ICollection<DepartmentStaff> DepartmentStaff { get; set; }
    }
}
