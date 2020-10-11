using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessTier.Requests
{
    public class AuthenticateRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public byte[] GetPasswordHash() => Encoding.ASCII.GetBytes(Password);
    }
}
