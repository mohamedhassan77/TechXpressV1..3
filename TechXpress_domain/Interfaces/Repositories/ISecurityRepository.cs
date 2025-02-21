using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress_domain.Entities;

namespace TechXpress_domain.Interfaces.Repositories
{
    public interface ISecurityRepository
    {
        Task<IEnumerable<ActiveSession>> GetActiveSessionsAsync(string userId);
        Task<ActiveSession> GetActiveSessionByIdAsync(string sessionId);
        Task TerminateSessionAsync(ActiveSession session);
        Task<IEnumerable<LoginHistoryEntry>> GetLoginHistoryAsync(string userId);
        Task SaveChangesAsync();
    }
}
