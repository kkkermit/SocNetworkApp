using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocNetworkApp.API.Models;

namespace SocNetworkApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> Login(string username, string password)
        {
            User user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.UserName == username);

            if (user == null)
            {
                return null;
            }

            // if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            // {
            //     return null;
            // }

            return user;
        }

        public async Task<User> Register(User user, string password)
        {
             CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            //  user.PasswordHash = passwordHash;
            //  user.PasswordSalt = passwordSalt;

             await _context.Users.AddAsync(user);
             await _context.SaveChangesAsync();

             return user;
        }

        public async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(HMACSHA512 hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(HMACSHA512 hmac = new HMACSHA512(passwordSalt))
            {
                byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (i < passwordHash.Length && computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }

                return true;
            } 
        }
    }
}