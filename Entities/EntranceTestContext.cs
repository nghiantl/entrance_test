using Microsoft.EntityFrameworkCore;
using System;

namespace WebApi.Entities
{
    public class EntranceTestContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Tokens> Tokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=178.128.109.9;Database=entrance_test;user=test01;password=PlsDoNotShareThePass123@");
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string passwrord { get; set; }
        public DateTime updatedAt { get; set; }
        public DateTime createdAt { get; set; }
    }

    public class Tokens
    {
        public int Id { get; set; }
        public int userId { get; set; }
        public string refreshToken { get; set; }
        public string expiresIn { get; set; }
        public DateTime updatedAt { get; set; }
        public DateTime createdAt { get; set; }
        public User user { get; set; }
    }
}
