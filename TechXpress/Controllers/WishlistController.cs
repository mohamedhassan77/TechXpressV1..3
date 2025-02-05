using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechXpress_domain.Entities;
using TechXpress_application.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using TechXpress.Repositories;

namespace TechXpress.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public WishlistController(IWishlistRepository wishlistRepository, UserManager<ApplicationUser> userManager)
        {
            _wishlistRepository = wishlistRepository;
            _userManager = userManager;
        }

        // GET: Wishlist
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var wishlist = await _wishlistRepository.GetByUserIdAsync(userId);
            return View(wishlist?.Products ?? new List<Product>());
        }

        // POST: Wishlist/AddToWishlist
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var userId = _userManager.GetUserId(User);
            await _wishlistRepository.AddProductToWishlistAsync(userId, productId);
            // Trigger wishlist updated event
            TempData["ToastMessage"] = "Product added to wishlist!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var userId = _userManager.GetUserId(User);
            await _wishlistRepository.RemoveProductFromWishlistAsync(userId, productId);
            // Trigger wishlist updated event
            TempData["ToastMessage"] = "Product removed from wishlist!";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> GetWishlistCount()
        {
            var userId = _userManager.GetUserId(User);
            var wishlist = await _wishlistRepository.GetByUserIdAsync(userId);
            var count = wishlist?.Products?.Count ?? 0;
            return Json(new { count });
        }
    }
}