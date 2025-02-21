using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechXpress_domain.Entities
{
    public class Shipping
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [StringLength(50)]
        public string Carrier { get; set; }

        [StringLength(100)]
        public string? TrackingNumber { get; set; }

        public DateTime? ShippedDate { get; set; }
        public DateTime EstimatedDelivery { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";
    }
}
