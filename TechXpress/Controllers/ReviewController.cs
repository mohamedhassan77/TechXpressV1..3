using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TechXpress.Models;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // GET: /Review/Create?productId=5
        [HttpGet]
        public IActionResult Create(int productId)
        {
            var model = new ReviewViewModel { ProductId = productId };
            return View(model);
        }

        // POST: /Review/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Pass all required arguments: userId, productId, comment, and rating
            var resultMessage = await _reviewService.AddReviewAsync(userId, model.ProductId, model.Comment, model.Rating);
            TempData["Message"] = resultMessage;
            return RedirectToAction("Details", "Product", new { id = model.ProductId });
        }

        // GET: /Review/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (review.UserId != userId)
                return Forbid();

            var model = new ReviewViewModel
            {
                Id = review.Id,
                ProductId = review.ProductId,
                Comment = review.Comment,
                Rating = review.Rating
            };
            return View(model);
        }

        // POST: /Review/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReviewViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            // Call UpdateReviewAsync with reviewId, comment, and rating
            var resultMessage = await _reviewService.UpdateReviewAsync(model.Id, model.Comment, model.Rating);
            TempData["Message"] = resultMessage;
            return RedirectToAction("Details", "Product", new { id = model.ProductId });
        }

        // POST: /Review/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Call DeleteReviewAsync with reviewId and userId
            var resultMessage = await _reviewService.DeleteReviewAsync(id, userId);
            TempData["Message"] = resultMessage;
            return RedirectToAction("Details", "Product", new { id = productId });
        }
    }

  
}
