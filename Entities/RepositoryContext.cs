﻿using Entities.Configuration;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override  void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountFriend>()
                .HasKey(s => new { s.AccountId, s.FriendId });
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountFriend> AccountFriends { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventParticipant> EventParticipants { get; set; }

    }
}