using System.Collections.Generic;
using TechXpress_domain.DTOs;

namespace TechXpress.Models
{
    public class AdminReviewPageViewModel
    {
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
        public IEnumerable<ReviewDashboardDto> RecentReviews { get; set; }
        public IEnumerable<UserProfileViewModel> UserProfiles { get; set; }
    }
}
