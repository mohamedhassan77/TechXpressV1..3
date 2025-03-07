using System.Collections.Generic;

namespace TechXpress.Models
{
    public class AdminDashboardViewModel
    {
        public AdminDashboardViewModel()
        {
            Users = new List<UserProfileViewModel>();
            Orders = new List<OrderDetailsViewModel>();
            RecentProducts = new List<ProductViewModel>();
            RecentCategories = new List<CategoryViewModel>();
            RecentReviews = new List<ReviewViewModel>();
        }

        public List<UserProfileViewModel> Users { get; set; }
        public List<OrderDetailsViewModel> Orders { get; set; }
        public List<ProductViewModel> RecentProducts { get; set; }
        public List<CategoryViewModel> RecentCategories { get; set; }
        public List<ReviewViewModel> RecentReviews { get; set; }
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}