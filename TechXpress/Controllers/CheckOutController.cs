using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechXpress_domain.Entities;
using TechXpress_application.Interfaces;

namespace TechXpress.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ICartRepository _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutController(ICartRepository cartRepository, UserManager<ApplicationUser> userManager)
        {
            _cartRepository = cartRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            return View(cart?.CartItems ?? new List<CartItem>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder()
        {
            var userId = _userManager.GetUserId(User);
            var cart = await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null || !cart.CartItems.Any())
            {
                TempData["ToastMessage"] = "Your cart is empty!";
                return RedirectToAction(nameof(Index));
            }

            // Process the order (e.g., save to database, send confirmation email, etc.)
            // Clear the cart after placing the order
            await _cartRepository.ClearCartAsync(userId);

            TempData["ToastMessage"] = "Order placed successfully!";
            return RedirectToAction("Index", "Home");
        }
    }
}