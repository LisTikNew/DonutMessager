using DonutMessager.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext()
    {
        Database.EnsureCreated(); // Команда внутри конструктора!
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
    internal DbSet<Contact> Contacts { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=Maoam901157;Database=DonutMessager");
    }
}