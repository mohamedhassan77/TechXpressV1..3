using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress_domain.Entities;

namespace TechXpress_domain.Interfaces.Services
{
    public interface ISecurityService
    {
        Task<IEnumerable<ActiveSession>> GetActiveSessionsAsync(string userId);
        Task<Result> TerminateSessionAsync(string userId, string sessionId);
        Task<IEnumerable<LoginHistoryEntry>> GetLoginHistoryAsync(string userId);
    }
}
public class Result
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}