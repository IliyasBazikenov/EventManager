using Entities.Configuration;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Entities
{
    public class RepositoryContext : IdentityDbContext<Account, AccountRole, Guid>
    {
        public RepositoryContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override  void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AccountFriend>()
               .HasKey(s => new { s.AccountId, s.FriendId });

            modelBuilder.Entity<Account>().ToTable("Accounts");
            modelBuilder.Entity<AccountRole>().ToTable("AccountRoles");

            modelBuilder.ApplyConfiguration(new AccountRoleConfiguration());
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new EventConfiguration());
        }

        public DbSet<AccountFriend> AccountFriends { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventParticipant> EventParticipants { get; set; }

    }
}
