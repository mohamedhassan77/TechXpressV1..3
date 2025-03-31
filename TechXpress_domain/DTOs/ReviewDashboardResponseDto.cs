namespace TechXpress_domain.DTOs
{
    public class ReviewDashboardResponseDto
        {
         public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
        public IEnumerable<ReviewDashboardDto> RecentReviews { get; set; }
        public IEnumerable<UserProfileDto> UserProfiles { get; set; }
    }
    

}
