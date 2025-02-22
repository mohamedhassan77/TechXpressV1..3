using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TechXpress.Models;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace TechXpress.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private readonly ILogger<CartController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;


        public CartController(ICartService cartService, IProductService productService, ILogger<CartController> logger, UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _productService = productService;
            _logger = logger;
                _userManager = userManager;

        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Use UserManager to get the user
                var user = await _userManager.GetUserAsync(User);
                var userId = user?.Id;

                if (userId == null)
                {
                    return View(new CartViewModel()); 
                }

                var cart = await _cartService.GetCartByUserIdAsync(userId);
                var viewModel = MapCartToViewModel(cart);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart");
                TempData["ErrorMessage"] = "Failed to load cart";
                return View(new CartViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var userId = user?.Id;
                
                var product = await _productService.GetByIdAsync(productId);
                if (product == null)
                    return NotFound("Product not found");

                if (quantity <= 0)
                    return BadRequest("Invalid quantity");

                // Check for existing cart
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                if (cart == null)
                {
                    cart = new Cart { UserId = userId };
                    await _cartService.CreateCartAsync(cart);
                }

                var message = await _cartService.AddToCartAsync(userId, null, productId, quantity);
                TempData["SuccessMessage"] = message;
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart");
                return Json(new { success = false, message = "Failed to add item to cart: " + ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var userId = user?.Id;
                int count = await _cartService.GetCartItemCountAsync(userId);
                return Json(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart count");
                return StatusCode(500, "An error occurred while getting the cart count.");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var userId = user?.Id; 
                var message = await _cartService.UpdateCartQuantityAsync(userId, productId, quantity);

                TempData["SuccessMessage"] = message;  
                return RedirectToAction(nameof(Index));  
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart quantity");
                TempData["ErrorMessage"] = "Failed to update quantity";
                return RedirectToAction(nameof(Index)); // Redirect to the cart index on error
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var userId = user?.Id;
                await _cartService.RemoveFromCartAsync(userId, productId);
                TempData["SuccessMessage"] = "Item removed from cart."; 
                return RedirectToAction(nameof(Index));  
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item from cart");
                TempData["ErrorMessage"] = "Failed to remove item";
                return RedirectToAction(nameof(Index));  
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var userId = user?.Id;
                 await _cartService.ClearCartAsync(userId);
                 return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart");
                TempData["ErrorMessage"] = "Failed to clear cart";
                return RedirectToAction(nameof(Index));
            }
        }

        private CartViewModel MapCartToViewModel(Cart cart)
        {
            return new CartViewModel
            {
                CartId = cart.Id,
                UserId = cart.UserId,
                Items = cart.CartItems.Select(ci => new CartItemViewModel
                {
                     Id = ci.ProductId,
                    Product = new ProductViewModel
                    {
                        Id = ci.Product.Id,
                        Name = ci.Product.Name,
                        Description = ci.Product.Description,
                        Price = ci.Product.Price,
                        DiscountPrice = ci.Product.DiscountPrice,
                        ImageUrl = ci.Product.ImageUrl,
                        IsFeatured = ci.Product.IsFeatured,
                        CreatedDate = ci.Product.CreatedDate,
                        UpdatedDate = ci.Product.UpdatedDate,
                        Tag = ci.Product.Tag,
                        Brand = ci.Product.Brand,
                        CategoryId = ci.Product.CategoryId,
                        StockQuantity = ci.Product.StockQuantity,
                        SKU = ci.Product.SKU,
                        Specifications = ci.Product.Specifications,
                        OldPrice = ci.Product.OldPrice,
                        ProductImages = ci.Product.ProductImages.ToList(),
                        Category = new CategoryViewModel
                        {
                            Id = ci.Product.Category.Id,
                            Name = ci.Product.Category.Name,
                            Description = ci.Product.Category.Description,
                            ImageUrl = ci.Product.Category.ImageUrl,
                            CreatedAt = ci.Product.Category.CreatedAt,
                            UpdatedAt = ci.Product.Category.UpdatedAt,
                            IsFeatured = false
                        }
                    },
                    Quantity = ci.Quantity,
                    PriceAtPurchase = ci.Product.Price
                }).ToList()?? new List<CartItemViewModel>(),
                ShippingCost = cart.ShippingCost,
                DiscountAmount = cart.DiscountAmount
            };
        }
    }
}
