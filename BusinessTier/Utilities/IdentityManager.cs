using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BusinessTier.Utilities
{
    public static class IdentityManager
    {
        public static string GetUserIdFromToken(string raw)
        {
            var bearerRemoved = raw.Replace("Bearer ", string.Empty);
            var token = new JwtSecurityTokenHandler().ReadJwtToken(bearerRemoved);
            return token.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
        }
        public static IEnumerable<string> GetRolesFromToken(string raw)
        {
            var bearerRemoved = raw.Replace("Bearer ", string.Empty);
            var token = new JwtSecurityTokenHandler().ReadJwtToken(bearerRemoved);
            return token.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x=>x.Value);
        }

        public static string GetUsernameFromToken(string raw)
        {
            var bearerRemoved = raw.Replace("Bearer ", string.Empty);
            var token = new JwtSecurityTokenHandler().ReadJwtToken(bearerRemoved);
            return token.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
        }
        public static string GenerateJwtToken(string fullName, string[] roles, Guid userId, string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Constants.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var permClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, userId.ToString()),
                new Claim(ClaimTypes.Name, fullName),
                new Claim(ClaimTypes.NameIdentifier, username),
            };
            if (roles != null && roles.Length > 0)
            {
                foreach (string role in roles)
                {
                    permClaims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var token = new JwtSecurityToken(Constants.Issuer,
                Constants.Issuer,
                permClaims,
                expires: DateTime.Now.AddDays(90),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static bool VerifyHashedPassword(byte[] hashedPassword, string password)
        {
            byte[] buffer4;
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            byte[] src = hashedPassword;
            if ((src.Length != 0x31) || (src[0] != 0))
            {
                return false;
            }
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);
            byte[] buffer3 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }

            return ByteArraysEqual(buffer3, buffer4);
        }

        public static bool ByteArraysEqual(byte[] b1, byte[] b2)
        {
            if (b1 == b2) return true;
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i]) return false;
            }
            return true;
        }

        public static byte[] HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                //Đã catch password = null ở ngoài
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return dst;
        }
    }
}
