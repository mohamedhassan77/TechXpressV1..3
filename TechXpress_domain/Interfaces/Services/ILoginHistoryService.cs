using TechXpress_domain.Entities;

namespace TechXpress_domain.Interfaces.Services
{
    public interface ILoginHistoryService
    {
        Task AddLoginEntryAsync(string userId, string deviceType, string location, string browser, string os);
        Task<IEnumerable<LoginHistoryEntry>> GetUserLoginHistoryAsync(string userId);
        Task UpdateCurrentSessionAsync(string userId, bool isCurrentSession);
    }
}