using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;
using TechXpress_domain.Interfaces.Repositories;

namespace TechXpress_application.Services
{
    public class LoginHistoryService : ILoginHistoryService
    {
        private readonly ILoginHistoryRepository _repository;

        public LoginHistoryService(ILoginHistoryRepository repository)
        {
            _repository = repository;
        }

        public async Task AddLoginEntryAsync(string userId, string deviceType, string location, string browser, string os)
        {
            var entry = new LoginHistoryEntry
            {
                UserId = userId,
                DeviceType = deviceType,
                Location = location,
                Browser = browser,
                OS = os,
                DateTime = DateTime.UtcNow,
                IsCurrentSession = true
            };

            await _repository.AddAsync(entry);
        }

        public async Task<IEnumerable<LoginHistoryEntry>> GetUserLoginHistoryAsync(string userId)
        {
            return await _repository.GetUserLoginHistoryAsync(userId);
        }

        public async Task UpdateCurrentSessionAsync(string userId, bool isCurrentSession)
        {
            await _repository.UpdateCurrentSessionAsync(userId, isCurrentSession);
        }
    }
}