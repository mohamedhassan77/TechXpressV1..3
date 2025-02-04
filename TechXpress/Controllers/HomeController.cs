using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TechXpress.Models;
using TechXpress.Data;
using TechXpress_application.Interfaces ;
using TechXpress_application.Services ;
using Microsoft.Extensions.Logging;
using TechXpress_application.Services;
using TechXpress.Repositories; // Assuming you have a Data folder for the DbContext or repository

namespace TechXpress.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository; 
        private readonly ICategoryRepository _categoryRepository; 

        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository , ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;


        }

        public async Task<IActionResult> Index()
        {
            // Fetch both featured products and categories before returning the view
            // Fetch products and categories sequentially
            var featuredProducts = await _productRepository.GetFeaturedProductsAsync();
            var categories = await _categoryRepository.GetAllAsync();

            ViewData["Categories"] = categories;
            return View(featuredProducts);
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
