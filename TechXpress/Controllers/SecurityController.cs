using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TechXpress_domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace TechXpress.Controllers
{
    [Authorize]
    public class SecurityController : Controller
    {
        private readonly ISecurityService _securityService;
        private readonly ILogger<SecurityController> _logger;

        public SecurityController(ISecurityService securityService, ILogger<SecurityController> logger)
        {
            _securityService = securityService;
            _logger = logger;
        }

        // GET: /Security/Sessions
        public async Task<IActionResult> Sessions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sessions = await _securityService.GetActiveSessionsAsync(userId);
            return View(sessions);
        }

        // POST: /Security/TerminateSession
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TerminateSession(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                TempData["ErrorMessage"] = "Invalid session ID.";
                return RedirectToAction(nameof(Sessions));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _securityService.TerminateSessionAsync(userId, sessionId);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
            }
            else
            {
                TempData["SuccessMessage"] = "Session terminated successfully.";
            }
            return RedirectToAction(nameof(Sessions));
        }

        // GET: /Security/LoginHistory
        public async Task<IActionResult> LoginHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var history = await _securityService.GetLoginHistoryAsync(userId);
            return View(history);
        }
    }
}
