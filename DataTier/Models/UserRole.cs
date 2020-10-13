using System;
using System.Collections.Generic;

namespace DataTier.Models
{
    public partial class UserRole
    {
        public UserRole()
        {
            Account = new HashSet<Account>();
        }

        public int Id { get; set; }
        public string RoleName { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<Account> Account { get; set; }
    }
}
