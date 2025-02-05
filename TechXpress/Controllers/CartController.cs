using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechXpress_domain.Entities;
using TechXpress_application.Interfaces;
using System.Threading.Tasks;
using System.Linq;

namespace TechXpress.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ICartRepository cartRepository, UserManager<ApplicationUser> userManager)
        {
            _cartRepository = cartRepository;
            _userManager = userManager;
        }

        // GET: Cart
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            return View(cart?.CartItems ?? new List<CartItem>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var userId = _userManager.GetUserId(User);
            await _cartRepository.AddProductToCartAsync(userId, productId, quantity);
            // Trigger cart updated event
            TempData["ToastMessage"] = "Product added to cart!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userId = _userManager.GetUserId(User);
            await _cartRepository.RemoveProductFromCartAsync(userId, productId);
            // Trigger cart updated event
            TempData["ToastMessage"] = "Product removed from cart!";
            return RedirectToAction(nameof(Index));
        }
        // POST: Cart/UpdateCartItemQuantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCartItemQuantity(int productId, int quantity)
        {
            var userId = _userManager.GetUserId(User);
            await _cartRepository.UpdateCartItemQuantityAsync(userId, productId, quantity);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var userId = _userManager.GetUserId(User);
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            var count = cart?.CartItems?.Count ?? 0;
            return Json(new { count });
        }
    }
}