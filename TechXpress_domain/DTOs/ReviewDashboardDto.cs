namespace TechXpress_domain.DTOs
{
    public class ReviewDashboardDto
        {
            public int Id { get; set; }
            public string ProductName { get; set; }
            public string UserName { get; set; }
            public int Rating { get; set; }
            public string Comment { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    

}
