namespace TechXpress_domain.DTOs
{
    public class UserProfileDto
    {
        public string UserId { get; set; }            
        public string UserName { get; set; }          
        public string Email { get; set; }            
        public string FirstName { get; set; }       
        public string LastName { get; set; }          
        public bool IsActive { get; set; }           
         public string PhoneNumber { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}
