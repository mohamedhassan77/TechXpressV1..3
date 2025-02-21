using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Repositories;
using TechXpress_infrastructure.Data;

namespace TechXpress_data.Repositories
{
    public class SecurityRepository : ISecurityRepository
    {
        private readonly TechXpress_context _context;

        public SecurityRepository(TechXpress_context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ActiveSession>> GetActiveSessionsAsync(string userId)
        {
            return await _context.ActiveSessions
                .Where(s => s.UserId == userId)
                .ToListAsync();
        }

        public async Task<ActiveSession> GetActiveSessionByIdAsync(string sessionId)
        {
            return await _context.ActiveSessions.FirstOrDefaultAsync(s => s.Id == sessionId);
        }

        public async Task TerminateSessionAsync(ActiveSession session)
        {
            _context.ActiveSessions.Remove(session);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<LoginHistoryEntry>> GetLoginHistoryAsync(string userId)
        {
            return await _context.LoginHistoryEntries
                .Where(h => h.UserId == userId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
