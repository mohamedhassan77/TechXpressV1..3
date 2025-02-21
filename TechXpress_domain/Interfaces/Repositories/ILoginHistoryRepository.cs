using TechXpress_domain.Entities;

namespace TechXpress_domain.Interfaces.Repositories
{
    public interface ILoginHistoryRepository
    {
        Task AddAsync(LoginHistoryEntry entry);
        Task<IEnumerable<LoginHistoryEntry>> GetUserLoginHistoryAsync(string userId);
        Task UpdateCurrentSessionAsync(string userId, bool isCurrentSession);
    }
}