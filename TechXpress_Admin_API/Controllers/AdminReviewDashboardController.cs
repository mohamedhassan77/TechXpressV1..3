using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechXpress_domain.Entities;
using TechXpress_domain.DTOs;
using TechXpress_domain.Interfaces.Services;
    
namespace TechXpress_Admin_API.Controllers
{
    [ApiController]
    [Route("api/admin/reviewdashboard")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminReviewDashboardController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminReviewDashboardController> _logger;
        private readonly IMapper _mapper;

        public AdminReviewDashboardController(
            IReviewService reviewService,
            IAdminService adminService,
            ILogger<AdminReviewDashboardController> logger,
            IMapper mapper)
        {
            _reviewService = reviewService;
            _adminService = adminService;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/admin/reviewdashboard
        [HttpGet]
        public async Task<IActionResult> GetReviewDashboardData()
        {
            try
            {
                IEnumerable<Review> reviews = await _reviewService.GetAllReviewsAsync() ?? Enumerable.Empty<Review>();
                int totalReviews = reviews.Count();
                double averageRating = totalReviews > 0 ? reviews.Average(r => r.Rating) : 0;

                var recentReviews = reviews
                    .OrderByDescending(r => r.Id)
                    .Take(5)
                    .Select(r => new ReviewDashboardDto
                    {
                        Id = r.Id,
                        ProductName = r.Product?.Name ?? "N/A",
                        UserName = r.ApplicationUser?.UserName ?? "N/A",
                        Rating = r.Rating,
                        Comment = r.Comment ?? string.Empty,
                        CreatedAt = r.CreatedAt
                    }).ToList();

                IEnumerable<UserProfile> users = await _adminService.GetAllUsersAsync() ?? Enumerable.Empty<UserProfile>();
                var userProfiles = _mapper.Map<IEnumerable<UserProfileDto>>(users).ToList();

                var dashboardDto = new ReviewDashboardResponseDto
                {
                    TotalReviews = totalReviews,
                    AverageRating = Math.Round(averageRating, 1),
                    RecentReviews = recentReviews,
                    UserProfiles = userProfiles
                };

                return Ok(dashboardDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading review dashboard data");
                return StatusCode(500, "Failed to load review dashboard data");
            }
        }
    }
}
