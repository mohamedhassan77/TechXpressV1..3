using Microsoft.EntityFrameworkCore;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Repositories;
using TechXpress_infrastructure.Data;

namespace TechXpress_infrastructure.Repositories
{
    public class LoginHistoryRepository : ILoginHistoryRepository
    {
        private readonly TechXpress_context _context;

        public LoginHistoryRepository(TechXpress_context context)
        {
            _context = context;
        }

        public async Task AddAsync(LoginHistoryEntry entry)
        {
            await _context.LoginHistoryEntries.AddAsync(entry);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<LoginHistoryEntry>> GetUserLoginHistoryAsync(string userId)
        {
            return await _context.LoginHistoryEntries
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.DateTime)
                .ToListAsync();
        }

        public async Task UpdateCurrentSessionAsync(string userId, bool isCurrentSession)
        {
            var currentSessions = await _context.LoginHistoryEntries
                .Where(h => h.UserId == userId && h.IsCurrentSession)
                .ToListAsync();

            foreach (var session in currentSessions)
            {
                session.IsCurrentSession = isCurrentSession;
            }

            await _context.SaveChangesAsync();
        }
    }
}