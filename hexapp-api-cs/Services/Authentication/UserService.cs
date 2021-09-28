using hexapp_api_cs.Models.Authentication;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hexapp_api_cs.Services.Authentication
{
    public class UserService
    {
        private readonly HexContext _context;

        public UserService()
        {
            _context = new HexContext();
        }

        public async Task<List<User>> Get() =>
            await _context.Users.ToListAsync();

        public async Task<User> Get(int id) =>
            await _context.Users.Where<User>(user => user.UserId == id).FirstOrDefaultAsync();

        public async Task<User> GetByUsername(string username) =>
            await _context.Users.Where<User>(user => user.Username.ToLower() == username.ToLower()).FirstOrDefaultAsync();

        public async Task<User> Create(UserCreate create)
        {
            var user = User.FromCreate(create);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async void Update(User original, UserUpdate update)
        {
            var user = User.FromUpdate(original, update);

            await _context.SaveChangesAsync();
        }
    }
}
