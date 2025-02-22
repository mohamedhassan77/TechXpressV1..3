using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress_domain.Entities;

namespace TechXpress_domain.Interfaces.Repositories
{
    public interface IUserProfileRepository
    {
        Task<IEnumerable<UserProfile>> GetAllAsync();  
        Task<UserProfile?> GetByIdAsync(string applicationUserId);
        Task AddAsync(UserProfile userProfile);
        Task UpdateAsync(UserProfile userProfile);
        Task DeleteAsync(string applicationUserId);
        Task<bool> ExistsAsync(string applicationUserId);
        Task<UserProfile?> GetByEmailAsync(string email);

        Task SaveChangesAsync();
    }
}
