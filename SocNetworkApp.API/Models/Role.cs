using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace SocNetworkApp.API.Models
{
    public class Role : IdentityRole<Guid>
    {
        public ICollection<UserRole> UserRoles { get; set; }
    }
}