using System.Collections.Generic;
using TechXpress_domain.Entities;

namespace TechXpress.Models
{
    public class AdminDashboardViewModel
    {
        public IEnumerable<UserProfile> Users { get; set; } = new List<UserProfile>();
        public IEnumerable<Order> Orders { get; set; } = new List<Order>();
        public int TotalUsers => Users.Count();
        public int TotalOrders => Orders.Count();
        public decimal TotalRevenue => Orders.Sum(o => o.Total);
        public int PendingOrders => Orders.Count(o => o.Status == "Pending");
    }
}
