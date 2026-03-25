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

        public async Task SaveUserInterests(string userId, IEnumerable<string> selectedInterestNames)
        {
            selectedInterestNames ??= Enumerable.Empty<string>();

            var interestNames = selectedInterestNames
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => name.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (interestNames.Count == 0)
            {
                return;
            }

            var currentUser = await _context.Users.OfType<User>().FirstOrDefaultAsync(x => x.Id == userId);

            if (currentUser is null)
            {
                throw new InvalidOperationException($"User with id '{userId}' was not found.");
            }

            var existingInterests = await _context.Interests
                .Where(interest => interestNames.Contains(interest.Name))
                .ToListAsync();

            var existingInterestNames = existingInterests
                .Select(interest => interest.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var newInterests = interestNames
                .Where(name => !existingInterestNames.Contains(name))
                .Select(name => new Interests { Name = name })
                .ToList();

            if (newInterests.Count > 0)
            {
                await _context.Interests.AddRangeAsync(newInterests);
                await _context.SaveChangesAsync();
                existingInterests.AddRange(newInterests);
            }

            var selectedInterestIds = existingInterests
                .Where(interest => interestNames.Contains(interest.Name, StringComparer.OrdinalIgnoreCase))
                .Select(interest => interest.Id)
                .Distinct()
                .ToList();

            var existingUserInterestIds = await _context.UserInterests
                .Where(userInterest => userInterest.UserId == userId && selectedInterestIds.Contains(userInterest.InterestId))
                .Select(userInterest => userInterest.InterestId)
                .ToListAsync();

            var userInterests = selectedInterestIds
                .Except(existingUserInterestIds)
                .Select(interestId => new UserInterests
                {
                    UserId = userId,
                    InterestId = interestId
                })
                .ToList();

            if (userInterests.Count == 0)
            {
                return;
            }

            await _context.UserInterests.AddRangeAsync(userInterests);
            await _context.SaveChangesAsync();
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


        public async Task SaveUserPhotos(string userId, IEnumerable<UserPhoto> photos)
        {
            var currentUser = await _context.Users.OfType<User>().FirstOrDefaultAsync(x => x.Id == userId);

            if (currentUser is null)
            {
                throw new InvalidOperationException($"User with id '{userId}' was not found.");
            }

            var photoEntities = photos.ToList();
            if (photoEntities.Count == 0)
            {
                return;
            }

            await _context.UserPhotos.AddRangeAsync(photoEntities);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserPhoto>> GetUserPhotos(string userId)
        {
            return await _context.UserPhotos
                .Where(photo => photo.UserId == userId)
                .OrderBy(photo => photo.SortOrder)
                .ThenBy(photo => photo.Created)
                .ToListAsync();
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
            currentUser.RomanticMen = user.RomanticMen;
            currentUser.RomanticWomen = user.RomanticWomen;
            currentUser.FriendshipMen = user.FriendshipMen;
            currentUser.FriendshipWomen = user.FriendshipWomen;
            currentUser.Sex = user.Sex;

            await _context.SaveChangesAsync();
        }
    }
}
