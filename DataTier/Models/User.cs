using System;
using System.Collections.Generic;

namespace DataTier.Models
{
    public partial class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Email { get; set; }
        public string UserNo { get; set; }
        public string FullName { get; set; }
        public string Phonenumber { get; set; }
        public string Address { get; set; }
        public string Photo { get; set; }
        public string RoleId { get; set; }
        public bool DelFlg { get; set; }
        public string InsBy { get; set; }
        public DateTime InsDatetime { get; set; }
        public string UpdBy { get; set; }
        public DateTime UpdDatetime { get; set; }

        public virtual Role Role { get; set; }
        public virtual Staff Staff { get; set; }
    }
}
