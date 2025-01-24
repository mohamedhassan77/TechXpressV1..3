using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TechXpress.Models;
using TechXpress.Data;
using TechXpress_infrastructure.Repositories; // Assuming you have a Data folder for the DbContext or repository

namespace TechXpress.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductRepository _productRepository; 

        public HomeController(ILogger<HomeController> logger, ProductRepository productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
            // Get featured products (e.g., top 3 featured products)
            var featuredProducts = await _productRepository.GetFeaturedProductsAsync(); // Await the async method
            return View(featuredProducts); // Passing products to the view
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
