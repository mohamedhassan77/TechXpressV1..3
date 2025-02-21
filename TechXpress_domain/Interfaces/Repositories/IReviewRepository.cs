using System.Collections.Generic;
using System.Threading.Tasks;
using TechXpress_domain.Entities;

namespace TechXpress_domain.Interfaces.Repositories
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId);
        Task<Review?> GetReviewByIdAsync(int id);
        Task AddReviewAsync(Review review);
        Task UpdateReviewAsync(Review review);
        Task DeleteReviewAsync(int id);
        Task<bool> UserHasReviewedProductAsync(string userId, int productId);
        Task SaveChangesAsync();
    }
}
