using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TechXpress_domain.Enums;

namespace TechXpress_domain.Entities
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(ApplicationUser))]
        public string UserId { get; set; } = null!;
        public ApplicationUser ApplicationUser { get; set; } = null!;

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")] 
        public OrderStatus Status { get; set; }

        [Required]
        [StringLength(100)]
        public string PaymentMethod { get; set; } = null!;

        [StringLength(50)]
        public string? CardType { get; set; }

        [StringLength(4)]
        public string? LastFourDigits { get; set; }

        [Required]
        public string TransactionId { get; set; } = null!;

        [StringLength(100)]
        public string? TrackingNumber { get; set; }

        [Url]
        public string? TrackingUrl { get; set; }

        public bool IsCancelled { get; set; } = false;
        public bool IsRefunded { get; set; } = false;

        public ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();

        [Required]
        public Shipping Shipping { get; set; } = null!;

        [NotMapped]
        public decimal Total => TotalPrice - Discount;

        [NotMapped]
        public ICollection<OrderItem> Items => OrderItems;
    }
}