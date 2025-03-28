using TechXpress_domain.DTOs;
using TechXpress_domain.Entities;

namespace TechXpress.Models
{
    public class DashboardData
    {
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingOrders { get; set; }
        public IEnumerable<ProductResponseDto> RecentProducts { get; set; }
        public IEnumerable<CategoryResponseDto> RecentCategories { get; set; }
        public IEnumerable<Review> RecentReviews { get; set; }
    }
}