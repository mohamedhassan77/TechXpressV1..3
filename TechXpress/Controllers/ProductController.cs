using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress_domain.Entities;
using TechXpress_infrastructure.Repositories;
 using TechXpress_application.Interfaces;
using Microsoft.AspNetCore.Identity;
using TechXpress.Repositories;
namespace TechXpress.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly IWishlistRepository _wishlistRepository;
        private readonly ICartRepository _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;


        public ProductController(ILogger<ProductController> logger, IProductRepository productRepository,
                                 IWishlistRepository wishlistRepository, ICartRepository cartRepository,
                                 UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _productRepository = productRepository;
            _wishlistRepository = wishlistRepository;
            _cartRepository = cartRepository;
            _userManager = userManager;
        }

        
        // GET: Product/Index
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetFeaturedProductsAsync();
            return View(products);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // GET: Product/GetProductDetails/5
        // This action is used to show detailed info (e.g., specifications and reviews) for a product.
        public async Task<IActionResult> GetProductDetails(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            // Example additional data; replace with actual service calls if needed.
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

            // Combine data into a view model 
            var viewModel = new ProductDetailsViewModel
            {
                Product = product,
                Specifications = string.Join("; ", specifications),
                Rating = rating,
                Reviews = string.Join(" | ", reviews)
            };

            return View(viewModel);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price")] Product product)
        {
            if (ModelState.IsValid)
            {
                await _productRepository.AddAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _productRepository.UpdateAsync(product);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating product with ID {id}");
                    // Optionally, you could add a ModelState error here and return the view.
                    return View(product);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product != null)
            {
                await _productRepository.DeleteAsync(product);
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Wishlist()
        {
            var userId = _userManager.GetUserId(User);
            var wishlist = await _wishlistRepository.GetByUserIdAsync(userId);
            return View(wishlist?.Products ?? new List<Product>());
        }

        public async Task<IActionResult> Cart()
        {
            var userId = _userManager.GetUserId(User);
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            return View(cart?.CartItems ?? new List<CartItem>());
        }

        [HttpPost]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var userId = _userManager.GetUserId(User);
            await _wishlistRepository.AddProductToWishlistAsync(userId, productId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var userId = _userManager.GetUserId(User);
            await _wishlistRepository.RemoveProductFromWishlistAsync(userId, productId);
            return RedirectToAction("Index");
        }

    


 
    }
}
       
     