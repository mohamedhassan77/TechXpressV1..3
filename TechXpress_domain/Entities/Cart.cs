using TechXpress_domain.Entities;

public class Cart
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public decimal TotalPrice { get; set; }
    
}