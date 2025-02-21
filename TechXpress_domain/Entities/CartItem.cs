using System.ComponentModel.DataAnnotations;

namespace TechXpress_domain.Entities
{
    public class CartItem
    {
        public int CartId { get; set; }
        public Cart? Cart { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}
