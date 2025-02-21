using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechXpress_domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceAtPurchase { get; set; }

        [NotMapped]
        public decimal Subtotal => Quantity * PriceAtPurchase;
    }
}
