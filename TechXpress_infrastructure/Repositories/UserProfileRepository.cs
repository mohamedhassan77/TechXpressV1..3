using Microsoft.EntityFrameworkCore;
using TechXpress.Repositories;
using TechXpress_domain.Entities;
using TechXpress_infrastructure.Data;

namespace TechXpress.Data.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly TechXpress_context _context;

        public UserProfileRepository(TechXpress_context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserProfile>> GetAllAsync()
        {
            return await _context.UserProfiles.ToListAsync();
        }

        public async Task<UserProfile> GetByIdAsync(string id)
        {
            return await _context.UserProfiles.FindAsync(id);
        }

        public async Task AddAsync(UserProfile userProfile)
        {
            await _context.UserProfiles.AddAsync(userProfile);
        }

        public async Task UpdateAsync(UserProfile userProfile)
        {
            _context.UserProfiles.Update(userProfile);
        }

        public async Task DeleteAsync(UserProfile userProfile)
        {
            _context.UserProfiles.Remove(userProfile);
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.UserProfiles.AnyAsync(e => e.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
