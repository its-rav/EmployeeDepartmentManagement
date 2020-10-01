using System;
using System.Collections.Generic;

namespace DataTier.Models
{
    public partial class Role
    {
        public Role()
        {
            User = new HashSet<User>();
        }

        public string RoleId { get; set; }
        public string RoleNm { get; set; }
        public bool DelFlg { get; set; }
        public string InsBy { get; set; }
        public DateTime InsDatetime { get; set; }
        public string UpdBy { get; set; }
        public DateTime UpdDatetime { get; set; }

        public virtual ICollection<User> User { get; set; }
    }
}
