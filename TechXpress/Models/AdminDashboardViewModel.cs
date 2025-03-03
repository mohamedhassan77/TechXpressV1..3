using System.Collections.Generic;

namespace TechXpress.Models
{
    public class AdminDashboardViewModel
    {
        public IEnumerable<UserProfileViewModel> Users { get; set; } = new List<UserProfileViewModel>();
        public IEnumerable<OrderDetailsViewModel> Orders { get; set; } = new List<OrderDetailsViewModel>();
        public IEnumerable<ProductViewModel> RecentProducts { get; set; } = new List<ProductViewModel>();
        public IEnumerable<CategoryViewModel> RecentCategories { get; set; } = new List<CategoryViewModel>();
        public IEnumerable<ReviewViewModel> RecentReviews { get; set; } = new List<ReviewViewModel>();

        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public decimal TotalRevenue { get; set; }
    }

}
