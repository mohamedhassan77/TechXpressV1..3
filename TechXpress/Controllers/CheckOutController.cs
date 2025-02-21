using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TechXpress.Models;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace TechXpress.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ICheckoutService _checkoutService;
        private readonly ICartService _cartService;
        private readonly UserManager<TechXpress_domain.Entities.ApplicationUser> _userManager;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(ICheckoutService checkoutService, ICartService cartService, UserManager<TechXpress_domain.Entities.ApplicationUser> userManager, ILogger<CheckoutController> logger)
        {
            _checkoutService = checkoutService;
            _cartService = cartService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var cart = await _cartService.GetCartByUserIdAsync(userId);

            var viewModel = new CheckoutViewModel
            {
                UserId = userId,
                CartItems = cart.CartItems, 
                Subtotal = cart.CartItems.Sum(ci => ci.Product.Price * ci.Quantity),
                ShippingCost = 0,
                DiscountAmount = 0,
                // Other fields can be populated as required
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(string paymentMethod, string transactionId)
        {
            var userId = _userManager.GetUserId(User);
            var result = await _checkoutService.ProcessCheckoutAsync(userId, paymentMethod, transactionId);
            if (result.StartsWith("Error"))
            {
                TempData["ErrorMessage"] = result;
                return RedirectToAction(nameof(Index));
            }
            TempData["SuccessMessage"] = result;
            return RedirectToAction("Index", "Home");
        }
    }
}
