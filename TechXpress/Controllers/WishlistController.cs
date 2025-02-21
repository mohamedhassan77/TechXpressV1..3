using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TechXpress.Models;
using TechXpress_domain.Interfaces.Services;
using TechXpress_domain.Entities;

namespace TechXpress.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly IWishlistService _wishlistService;
        private readonly UserManager<TechXpress_domain.Entities.ApplicationUser> _userManager;

        public WishlistController(IWishlistService wishlistService, UserManager<TechXpress_domain.Entities.ApplicationUser> userManager)
        {
            _wishlistService = wishlistService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var wishlist = await _wishlistService.GetWishlistAsync(userId);
            var viewModel = new WishlistViewModel
            {
                UserId = userId,
                WishlistItems = wishlist?.WishlistItems != null ? new System.Collections.Generic.List<WishlistItem>(wishlist.WishlistItems) : new System.Collections.Generic.List<WishlistItem>(),
                TotalItems = wishlist?.WishlistItems?.Count ?? 0,
                Items = wishlist?.WishlistItems != null ? new System.Collections.Generic.List<WishlistItem>(wishlist.WishlistItems) : new System.Collections.Generic.List<WishlistItem>()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var userId = _userManager.GetUserId(User);
            var message = await _wishlistService.AddToWishlistAsync(userId, productId);
            TempData["ToastMessage"] = message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var userId = _userManager.GetUserId(User);
            var message = await _wishlistService.RemoveFromWishlistAsync(userId, productId);
            TempData["ToastMessage"] = message;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetWishlistCount()
        {
            var userId = _userManager.GetUserId(User);
            var wishlist = await _wishlistService.GetWishlistAsync(userId);
            int count = wishlist?.WishlistItems?.Count ?? 0;
            return Json(new { count });
        }
    }
}
