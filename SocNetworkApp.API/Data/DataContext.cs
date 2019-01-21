using Microsoft.EntityFrameworkCore;
using SocNetworkApp.API.Models;

namespace SocNetworkApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }

        public DbSet<Photo> Photos { get; set; }
    }
}