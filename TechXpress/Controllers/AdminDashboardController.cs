using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TechXpress.Models;
using TechXpress_domain.DTOs;
using System.Collections.Generic;

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
            var token = HttpContext.Session.GetString("AdminToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "No token found. Please log in as admin.";
                return RedirectToAction("Login", "Account");
            }
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

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
