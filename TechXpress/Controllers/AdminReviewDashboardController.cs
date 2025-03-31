using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;
using TechXpress.Models;
using TechXpress_domain.DTOs;

namespace TechXpress.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    public class AdminReviewDashboardController : Controller
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

        // GET: /AdminReviewDashboard/Index
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                IEnumerable<Review> reviews = await _reviewService.GetAllReviewsAsync() ?? Enumerable.Empty<Review>();
                IEnumerable<UserProfile> users = await _adminService.GetAllUsersAsync() ?? Enumerable.Empty<UserProfile>();

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

                var userProfiles = _mapper.Map<IEnumerable<UserProfileViewModel>>(users).ToList();

                var compositeModel = new AdminReviewPageViewModel
                {
                    TotalReviews = totalReviews,
                    AverageRating = Math.Round(averageRating, 1),
                    RecentReviews = recentReviews,
                    UserProfiles = userProfiles
                };

                return View(compositeModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading composite review dashboard data.");
                TempData["ErrorMessage"] = "Failed to load review dashboard data.";
                return View(new AdminReviewPageViewModel
                {
                    RecentReviews = new List<ReviewDashboardDto>(),
                    UserProfiles = new List<UserProfileViewModel>()
                });
            }
        }
    }
}
