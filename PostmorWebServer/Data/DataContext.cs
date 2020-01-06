﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PostmorWebServer.Data.Entities;

namespace PostmorWebServer.Data
{
    public class DataContext : IdentityDbContext<User, IdentityRole<int>,int>
    {
        public DbSet<Letter> Letters { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var converter = new ValueConverter<string[], string>(
                v => string.Join(";", v),
                v => v.Split(";", StringSplitOptions.RemoveEmptyEntries).Select(val => val).ToArray());
            base.OnModelCreating(builder);
            builder.Entity<User>()
                .HasIndex(u => u.Address)
                .IsUnique();
            builder.Entity<Letter>()
                .Property(e => e.Message)
                .HasConversion(converter);

        }
    }

}
