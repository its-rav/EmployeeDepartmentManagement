using System;
using System.Collections.Generic;

namespace DataTier.Models
{
    public partial class Staff
    {
        public int StaffId { get; set; }
        public string DepartmentId { get; set; }
        public bool DelFlg { get; set; }
        public string InsBy { get; set; }
        public DateTime InsDatetime { get; set; }
        public string UpdBy { get; set; }
        public DateTime UpdDatetime { get; set; }

        public virtual Department Department { get; set; }
        public virtual User StaffNavigation { get; set; }
    }
}
