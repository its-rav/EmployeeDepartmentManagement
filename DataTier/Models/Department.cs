using System;
using System.Collections.Generic;

namespace DataTier.Models
{
    public partial class Department
    {
        public Department()
        {
            Staff = new HashSet<Staff>();
        }

        public string DepartmentId { get; set; }
        public string DepartmentNm { get; set; }
        public string Hotline { get; set; }
        public string RoomNum { get; set; }
        public bool DelFlg { get; set; }
        public string InsBy { get; set; }
        public DateTime InsDatetime { get; set; }
        public string UpdBy { get; set; }
        public DateTime UpdDatetime { get; set; }

        public virtual ICollection<Staff> Staff { get; set; }
    }
}
