namespace TechXpress_domain.Entities
{
    public class ApplicationUser : Microsoft.AspNetCore.Identity.IdentityUser 
    {
        public string Id { get; set; }  //  the primary key
        public string UserName { get; set; }

        // Navigation property
        public UserProfile UserProfile { get; set; }

    }
}
