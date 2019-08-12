using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocNetworkApp.API.Models;

namespace SocNetworkApp.API.Data
{
    public class DataContext : IdentityDbContext<User, Role, Guid, 
        IdentityUserClaim<Guid>, UserRole, IdentityUserLogin<Guid>, 
        IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new {ur.UserId, ur.RoleId});

                userRole.HasOne(ur => ur.Role)
                        .WithMany(r => r.UserRoles)
                        .HasForeignKey(ur => ur.RoleId)
                        .IsRequired();

                userRole.HasOne(ur => ur.User)
                        .WithMany(u => u.UserRoles)
                        .HasForeignKey(ur => ur.UserId)
                        .IsRequired();
            });

            builder.Entity<Like>()
                   .HasKey(k => new {k.LikeeId, k.LikerId});

            builder.Entity<Like>()
                   .HasOne(u => u.Likee)
                   .WithMany(u => u.Likers)
                   .HasForeignKey(u => u.LikeeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>()
                   .HasOne(u => u.Liker)
                   .WithMany(u => u.Likees)
                   .HasForeignKey(u => u.LikerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                   .HasOne(m => m.Sender)
                   .WithMany(u => u.MessagesSent)
                   .OnDelete(DeleteBehavior.Restrict);
       
            builder.Entity<Message>()
                   .HasOne(m => m.Recipient)
                   .WithMany(u => u.MessagesRecived)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Photo>().HasQueryFilter(p => p.IsApproved);
        }
    }
}