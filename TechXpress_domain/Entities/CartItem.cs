using TechXpress_domain.Entities;

public class CartItem
{

    public int ProductId { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public string UserId { get; set; }
    public ApplicationUser applicationUser { get; set; }
    public string CartId { get; set; }
    public Cart Cart { get; set; }
}