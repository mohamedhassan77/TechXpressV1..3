namespace TechXpress_domain.DTOs
{
    public class DashboardData
    {
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingOrders { get; set; }
        public List<ProductResponseDto> RecentProducts { get; set; }
        public List<CategoryResponseDto> RecentCategories { get; set; }
        public List<ReviewDashboardDto> RecentReviews { get; set; }
    }
}
