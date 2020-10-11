using System;
using System.Collections.Generic;

namespace DataTier.Models
{
    public partial class DepartmentStaff
    {
        public Guid AccountId { get; set; }
        public string DepartmentId { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdateBy { get; set; }

        public virtual Account Account { get; set; }
        public virtual Department Department { get; set; }
    }
}
