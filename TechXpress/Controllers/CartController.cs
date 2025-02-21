using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TechXpress.Models;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace TechXpress.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, IProductService productService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _productService = productService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                var viewModel = MapCartToViewModel(cart);
                return View(viewModel);
            }
            catch (System.Exception ex)
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
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var product = await _productService.GetByIdAsync(productId);
                if (product == null)
                    return NotFound();

                if (quantity <= 0)
                    return BadRequest("Invalid quantity");

                var message = await _cartService.AddToCartAsync(userId, null, productId, quantity);
                TempData["SuccessMessage"] = message;
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart");
                return Json(new { success = false, message = "Failed to add item to cart" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var message = await _cartService.UpdateCartQuantityAsync(userId, productId, quantity);
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                var viewModel = MapCartToViewModel(cart);
                return Json(new
                {
                    success = true,
                    total = viewModel.Total,
                    itemCount = viewModel.ItemCount
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error updating cart quantity");
                return Json(new { success = false, message = "Failed to update quantity" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var message = await _cartService.RemoveFromCartAsync(userId, productId);
                TempData["SuccessMessage"] = message;
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error removing item from cart");
                return Json(new { success = false, message = "Failed to remove item" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var message = await _cartService.ClearCartAsync(userId);
                TempData["SuccessMessage"] = message;
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
                    // Since domain CartItem might not have its own Id, we can use ProductId as a surrogate key here.
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
                }).ToList(),
                ShippingCost = 0,
                DiscountAmount = 0
            };
        }
    }
}
