using LikesAndSwipes.Data;
using LikesAndSwipes.Models;
using Microsoft.EntityFrameworkCore;

namespace LikesAndSwipes.Repositories
{
    public class DataRepository
    {
        private readonly ApplicationDbContext _context;

        public DataRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LocationEntity> SaveUserLocation(LocationEntity location)
        {
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();

            return location;
        }

        public async Task<List<Interests>> GetAllInterests()
        {
            var result = await _context.Interests.ToListAsync();

            return result;
        }

        public async Task SaveUserFirstName(User user)
        {
            var currentUser = await _context.Users.OfType<User>().FirstOrDefaultAsync(x => x.Id == user.Id);

            if (currentUser is null)
            {
                throw new InvalidOperationException($"User with id '{user.Id}' was not found.");
            }

            currentUser.FirstName = user.FirstName;

            await _context.SaveChangesAsync();
        }

        public async Task SaveUserRegistrationData(User user)
        {
            var currentUser = await _context.Users.OfType<User>().FirstOrDefaultAsync(x => x.Id == user.Id);

            if (currentUser is null)
            {
                throw new InvalidOperationException($"User with id '{user.Id}' was not found.");
            }

            currentUser.FirstName = user.FirstName;
            currentUser.Age = user.Age;
            currentUser.UserName = user.FirstName;
            currentUser.Sex = user.Sex;

            await _context.SaveChangesAsync();
        }
    }
}
