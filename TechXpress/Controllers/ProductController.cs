using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TechXpress_domain.Entities;
using TechXpress_infrastructure.Repositories;
using System.Linq;

namespace TechXpress.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ProductRepository _productRepository;

        public ProductController(ILogger<ProductController> logger, ProductRepository productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
        }

        // GET: Product/Index
        public IActionResult Index()
        {
            var products = _productRepository.GetFeaturedProductsAsync();
            return View(products);
        }

        // GET: Product/Details/5
        public IActionResult Details(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        public ActionResult GetProductDetails(int productId)
        {
            var product =_productRepository.GetById(productId);

            // Assume these are retrieved from another service or predefined data
            var specifications = new List<string>
    {
        "Color: Red",
        "Size: Medium",
        "Weight: 1.5 kg"
    };

            var reviews = new List<string>
    {
        "Great product!",
        "Highly recommend it.",
        "Would buy again."
    };

            var rating = 4.5f; // Example rating

            // Convert specs and reviews into a format that can be passed to the modal
            var specificationsString = string.Join("; ", specifications);
            var reviewsString = string.Join(" | ", reviews);

            return View(new
            {
                Product = product,
                Specifications = specificationsString,
                Rating = rating,
                Reviews = reviewsString
            });
        }
        // GET: Product/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Description,Price")] Product product)
        {
            if (ModelState.IsValid)
            {
                _productRepository.Add(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Product/Edit/5
        public IActionResult Edit(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Description,Price")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _productRepository.Update(product);
                }
                catch
                {
                    // Handle any errors here, like when the product is not found
                    _logger.LogError($"Error updating product with ID {id}");
                    return View(product);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Product/Delete/5
        public IActionResult Delete(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _productRepository.GetById(id);
            if (product != null)
            {
                _productRepository.Delete(product);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
