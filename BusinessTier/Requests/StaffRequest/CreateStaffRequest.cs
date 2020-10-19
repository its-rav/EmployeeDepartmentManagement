using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessTier.Requests.StaffRequest
{
    public class CreateStaffRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Photo { get; set; }
        public int RoleId { get; set; }
    }
}
