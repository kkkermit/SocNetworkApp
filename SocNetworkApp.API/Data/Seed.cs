using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using SocNetworkApp.API.Models;
using Microsoft.AspNetCore.Identity;

namespace SocNetworkApp.API.Data
{
    public class Seed
    {
        private readonly UserManager<User> _userNamager;
        public RoleManager<Role> _roleManager;

        public Seed(UserManager<User> userNamager, RoleManager<Role> roleManager)
        {
            _userNamager = userNamager;
            _roleManager = roleManager;
        }

        public void SeedUsers()
        {
            if (!_userNamager.Users.Any())
            {
                string userData = File.ReadAllText("Data/UserSeedData.json");
                List<User> users = JsonConvert.DeserializeObject<List<User>>(userData);

                List<Role> roles = new List<Role>
                {
                    new Role { Name = "Member" },
                    new Role { Name = "Admin" },
                    new Role { Name = "Moderator" },
                    new Role { Name = "VIP" }
                };

                foreach (Role role in roles)
                {
                    _roleManager.CreateAsync(role).Wait();
                }

                foreach (User user in users)
                {
                    if (user.Photos.FirstOrDefault() != null)
                    {
                        user.Photos.SingleOrDefault().IsApproved = true;
                    }

                    _userNamager.CreateAsync(user, "password").Wait();
                    _userNamager.AddToRoleAsync(user, "Member").Wait();
                }

                User adminUser = new User
                {
                    UserName = "Admin"
                };

                IdentityResult result = _userNamager.CreateAsync(adminUser, "password").Result;

                if (result.Succeeded)
                {
                    User admin = _userNamager.FindByNameAsync("Admin").Result;
                    _userNamager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" }).Wait();
                }
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (HMACSHA512 hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}