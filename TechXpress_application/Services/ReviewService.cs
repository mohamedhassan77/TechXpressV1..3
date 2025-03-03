using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Repositories;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress_application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IProductRepository _productRepository;

        public ReviewService(IReviewRepository reviewRepository, IProductRepository productRepository)
        {
            _reviewRepository = reviewRepository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId)
        {
            return await _reviewRepository.GetReviewsByProductIdAsync(productId);
        }

        // NEW METHOD: Returns all reviews.
        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            // Assumes your repository has a GetAllAsync() method.
            return await _reviewRepository.GetAllAsync();
        }

        public async Task<Review?> GetReviewByIdAsync(int id)
        {
            return await _reviewRepository.GetReviewByIdAsync(id);
        }

        public async Task<string> AddReviewAsync(string userId, int productId, string comment, int rating)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                return "Product not found.";

            if (await _reviewRepository.UserHasReviewedProductAsync(userId, productId))
                return "You have already reviewed this product.";

            var review = new Review
            {
                UserId = userId,
                ProductId = productId,
                Comment = comment,
                Rating = rating,
                CreatedAt = DateTime.UtcNow
            };

            await _reviewRepository.AddReviewAsync(review);
            await _reviewRepository.SaveChangesAsync();

            return "Review added successfully.";
        }

        public async Task<string> UpdateReviewAsync(int reviewId, string comment, int rating)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(reviewId);
            if (review == null)
                return "Review not found.";

            review.Comment = comment;
            review.Rating = rating;

            await _reviewRepository.UpdateReviewAsync(review);
            await _reviewRepository.SaveChangesAsync();

            return "Review updated successfully.";
        }

        public async Task<string> DeleteReviewAsync(int reviewId, string userId)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(reviewId);
            if (review == null)
                return "Review not found.";

            if (review.UserId != userId)
                return "You can only delete your own review.";

            await _reviewRepository.DeleteReviewAsync(reviewId);
            await _reviewRepository.SaveChangesAsync();

            return "Review deleted successfully.";
        }
    }
}
