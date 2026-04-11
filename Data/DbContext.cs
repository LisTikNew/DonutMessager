using DonutMessager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace DonutMessager
{
    public class AppDbContext : DbContext
    {
        internal DbSet<User> Users { get; set; }
        internal DbSet<Contact> Contacts { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=Maoam901157; Database=DonutMessager");
        }
    }
}
