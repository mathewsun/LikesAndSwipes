using LikesAndSwipes.Data;
using LikesAndSwipes.Models;

namespace LikesAndSwipes.Repositories
{
    public class DataRepository
    {
        private ApplicationDbContext _context;

        public DataRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LocationEntity> SaveUserLocation(LocationEntity location)
        {
            _context.Locations.Add(location);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }

            long generatedId = location.Id;

            //LocationEntity savedLocation = await _context.Locations.FirstAsync(x => x.Id == generatedId);

            return location; // savedLocation;
        }
    }
}
