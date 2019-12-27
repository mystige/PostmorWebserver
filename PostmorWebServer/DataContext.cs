using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PostmorWebServer
{
    public class DataContext : IdentityDbContext<User, IdentityRole<int>,int>
    {
        public DbSet<Letter> Letters { get; set; }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {             
            base.OnModelCreating(builder);
            builder.Entity<User>()
                .HasIndex(u => u.Adress)
                .IsUnique();
        }
    }

}
