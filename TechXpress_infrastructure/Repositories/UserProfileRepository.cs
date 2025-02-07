using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechXpress_application.Interfaces;
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
            try
            {
                return await _context.UserProfiles.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving user profiles.", ex);
            }
        }

        public async Task<UserProfile> GetByIdAsync(string id)
        {
            try
            {
                return await _context.UserProfiles.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving the user profile with ID {id}.", ex);
            }
        }

        public async Task AddAsync(UserProfile userProfile)
        {
            try
            {
                await _context.UserProfiles.AddAsync(userProfile);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while adding the user profile.", ex);
            }
        }

        public async Task UpdateAsync(UserProfile userProfile)
        {
            try
            {
                _context.UserProfiles.Update(userProfile);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while updating the user profile.", ex);
            }
        }

        public async Task DeleteAsync(UserProfile userProfile)
        {
            try
            {
                _context.UserProfiles.Remove(userProfile);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while deleting the user profile.", ex);
            }
        }

        public async Task<bool> ExistsAsync(string id)
        {
            try
            {
                return await _context.UserProfiles.AnyAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while checking if the user profile with ID {id} exists.", ex);
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while saving changes to the database.", ex);
            }
        }
    }
}