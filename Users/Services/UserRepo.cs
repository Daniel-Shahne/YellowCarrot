using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YellowCarrot.Users.Data;
using YellowCarrot.Users.Models;

namespace YellowCarrot.Users.Services
{
    internal class UserRepo
    {
        private UsersDbContext context;

        public UserRepo(UsersDbContext context)
        {
            this.context = context;
        }

        /* Returns a user (or null) if username match found */
        public async Task<User?> GetUserByUsernameAsync(string userName)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Username == userName);
        }

        /* Creates a new user */
        public async Task CreateUserAsync(User newUser)
        {
            await context.Users.AddAsync(newUser);
        }

        /* Returns a list of all usernames */
        public async Task<List<string>> GetUsernamesAsync()
        {
            return await context.Users.Select(u => u.Username).ToListAsync();
        }
    }
}
