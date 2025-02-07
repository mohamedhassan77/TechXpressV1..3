using TechXpress_domain.Entities;

public class Cart
{
    public string Id { get; set; } = Guid.NewGuid().ToString(); 
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public decimal TotalPrice { get; set; }
    
}