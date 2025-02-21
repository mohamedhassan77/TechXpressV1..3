using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Repositories;
using TechXpress_infrastructure.Data;

namespace TechXpress_infrastructure.Repositories
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
            return await _context.UserProfiles
                .Include(up => up.Addresses)
                .ToListAsync();
        }

        public async Task<UserProfile?> GetByIdAsync(string applicationUserId)
        {
            return await _context.UserProfiles
                .Include(up => up.Addresses)
                .FirstOrDefaultAsync(up => up.ApplicationUserId == applicationUserId);
        }

        public async Task AddAsync(UserProfile userProfile)
        {
            if (userProfile.Addresses == null)
            {
                userProfile.Addresses = new List<Address>();
            }
            await _context.UserProfiles.AddAsync(userProfile);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserProfile userProfile)
        {
            _context.UserProfiles.Update(userProfile);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string applicationUserId)
        {
            var userProfile = await GetByIdAsync(applicationUserId);
            if (userProfile == null)
            {
                throw new KeyNotFoundException($"UserProfile with ID {applicationUserId} not found.");
            }

            _context.UserProfiles.Remove(userProfile);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string applicationUserId)
        {
            return await _context.UserProfiles.FindAsync(applicationUserId) != null;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
