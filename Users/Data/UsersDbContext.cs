using EntityFrameworkCore.EncryptColumn.Extension;
using EntityFrameworkCore.EncryptColumn.Interfaces;
using EntityFrameworkCore.EncryptColumn.Util;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YellowCarrot.Users.Models;

namespace YellowCarrot.Users.Data
{
    internal class UsersDbContext : DbContext
    {
        private IEncryptionProvider encryptionProvider = new GenerateEncryptionProvider("123456789_123456789_1234");
        
        public UsersDbContext()
        {

        }

        public UsersDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder ob)
        {
            ob.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=YellowCarrotUsersDb;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.UseEncryption(encryptionProvider);

            mb.Entity<User>().HasData(
                new User()
                {
                    UserId = 1,
                    Username = "user",
                    Password = "password"
                },
                new User()
                {
                    UserId = 2,
                    Username = "user2",
                    Password = "password2"
                }
            );
        }
    }
}
