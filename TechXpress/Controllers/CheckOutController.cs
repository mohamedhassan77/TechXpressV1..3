using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TechXpress.Models;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;
using TechXpress_application.Services;
using System.Security.Claims;

namespace TechXpress.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ICheckoutService _checkoutService;
        private readonly ICartService _cartService;
        private readonly IUserProfileService _userProfileService;
        private readonly ILogger<CheckoutController> _logger;
        

        public CheckoutController(
            ICheckoutService checkoutService,
            ICartService cartService,
            UserManager<ApplicationUser> userManager,
            ILogger<CheckoutController> logger,
            IUserProfileService userProfile)
        {
            _checkoutService = checkoutService;
            _cartService = cartService;
             _logger = logger;
            _userProfileService = userProfile;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cart = await _cartService.GetCartByUserIdAsync(userId);

            if (cart == null || !cart.CartItems.Any())
            {
                TempData["InfoMessage"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }
            var user = await _userProfileService.GetUserProfileAsync(userId);

         
            var defaultAdress = _userProfileService.GetDefaultShippingAddressAsync(userId).Result;
            // Calculate OrderTotal (Subtotal - DiscountAmount + ShippingCost + Tax)
            var subtotal = cart.CartItems.Sum(ci => ci.Product.Price * ci.Quantity);
            var shippingCost = 15;
            var tax = 5;
            var discountAmount = 0;
            var orderTotal = subtotal - discountAmount + shippingCost + tax;
            
            var viewModel = new CheckoutViewModel
            {
                UserId = userId,
                CartItems = cart.CartItems,
                Subtotal = subtotal,
                ShippingCost = shippingCost,
                DiscountAmount = discountAmount,
                Tax = tax,
                OrderTotal = orderTotal,
                AgreeToTerms = false,
                // Set default shipping & billing addresses to the default dummy address
                ShippingAddress = defaultAdress ,
                BillingAddress = defaultAdress ,
                PaymentMethod = "CreditCard",
                ShippingMethod = "Standard",
                SpecialInstructions = "",
                IsGiftWrapping = false,
                GiftMessage = "",
                SavedAddresses = user.Addresses,
             };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(int productId, int quantity, string paymentMethod, string transactionId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve the user's cart
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                // If no cart exists, you might want to create one.
                cart = new Cart { UserId = userId };
                // Optionally persist the new cart here if necessary.
            }

            // Check if the product already exists in the cart
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Product.Id == productId);
            if (cartItem == null)
            {
                // Directly add the product with the specified quantity since cartItem is null
                var addResult = await _cartService.AddToCartAsync(userId, "", productId, quantity);
                if (addResult.StartsWith("Error"))
                {
                    TempData["ErrorMessage"] = addResult;
                    return RedirectToAction("Index", "Cart");
                }
            }
            else
            {
                // Update the existing product's quantity (increase by the specified amount)
                cartItem.Quantity += quantity;
                var updateResult = await _cartService.UpdateCartQuantityAsync(userId, productId, cartItem.Quantity);
                if (updateResult.StartsWith("Error"))
                {
                    TempData["ErrorMessage"] = updateResult;
                    return RedirectToAction("Index", "Cart");
                }
            }

            // Refresh the cart (if needed)
            cart = await _cartService.GetCartByUserIdAsync(userId);

            try
            {
                // Proceed to checkout with all products in the cart
                var result = await _checkoutService.ProcessCheckoutAsync(userId, paymentMethod, transactionId);
                if (result.StartsWith("Error"))
                {
                    TempData["ErrorMessage"] = result;
                    return RedirectToAction(nameof(Index));
                }
                TempData["SuccessMessage"] = result;
                return RedirectToAction("Index", "Order");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing checkout for user {UserId}", userId);
                TempData["ErrorMessage"] = "An unexpected error occurred during checkout. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }


    }
}
