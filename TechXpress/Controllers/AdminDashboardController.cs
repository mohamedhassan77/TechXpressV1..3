using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TechXpress.Models;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using System.Collections.Generic;
using TechXpress_domain.DTOs;

namespace TechXpress.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AdminDashboardController(IHttpClientFactory httpClientFactory, IConfiguration configuration, IMapper mapper)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _mapper = mapper;
        }

        // GET: /AdminDashboard/Index
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("AdminApiClient");

            // Retrieve token from session (or wherever you store it)
            var token = HttpContext.Session.GetString("AdminToken");

            // If token is not found, redirect to login.
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "No token found. Please log in as admin.";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                // Attach the token to the API request header.
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync("api/admin/dashboard");
            if (response.IsSuccessStatusCode)
            {
                var dashboardData = await response.Content.ReadFromJsonAsync<DashboardData>();

                var viewModel = new AdminDashboardViewModel
                {
                    TotalUsers = dashboardData.TotalUsers,
                    TotalOrders = dashboardData.TotalOrders,
                    TotalRevenue = dashboardData.TotalRevenue,
                    PendingOrders = dashboardData.PendingOrders,
                    RecentProducts = _mapper.Map<List<ProductViewModel>>(dashboardData.RecentProducts),
                    RecentCategories = _mapper.Map<List<CategoryViewModel>>(dashboardData.RecentCategories),
                    RecentReviews = _mapper.Map<List<ReviewViewModel>>(dashboardData.RecentReviews)
                };

                return View(viewModel);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Failed to load dashboard data from API. Status: {response.StatusCode}. Error: {errorContent}";
                return View(new AdminDashboardViewModel());
            }
        }
    }
}
