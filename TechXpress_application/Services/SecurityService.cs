using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress_domain;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Repositories;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress_services
{
    public class SecurityService : ISecurityService
    {
        private readonly ISecurityRepository _securityRepository;

        public SecurityService(ISecurityRepository securityRepository)
        {
            _securityRepository = securityRepository;
        }

        public async Task<IEnumerable<ActiveSession>> GetActiveSessionsAsync(string userId)
        {
            return await _securityRepository.GetActiveSessionsAsync(userId);
        }

        public async Task<Result> TerminateSessionAsync(string userId, string sessionId)
        {
            try
            {
                var session = await _securityRepository.GetActiveSessionByIdAsync(sessionId);
                if (session == null || session.UserId != userId)
                {
                    return new Result { Success = false, Message = "Session not found or unauthorized." };
                }

                await _securityRepository.TerminateSessionAsync(session);
                await _securityRepository.SaveChangesAsync();
                return new Result { Success = true, Message = "Session terminated successfully." };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = ex.Message };
            }
        }

        public async Task<IEnumerable<LoginHistoryEntry>> GetLoginHistoryAsync(string userId)
        {
            return await _securityRepository.GetLoginHistoryAsync(userId);
        }
    }
}
