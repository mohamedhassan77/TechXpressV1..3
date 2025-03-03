using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress_domain.Entities;

namespace TechXpress_domain.Interfaces.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId);
        Task<Review?> GetReviewByIdAsync(int id);
        Task<string> AddReviewAsync(string userId, int productId, string comment, int rating);
        Task<string> UpdateReviewAsync(int reviewId, string comment, int rating);
        Task<string> DeleteReviewAsync(int reviewId, string userId);
        Task<IEnumerable<Review>> GetAllReviewsAsync();

    }
}
