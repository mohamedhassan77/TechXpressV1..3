using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TechXpress_domain.Entities;

namespace TechXpress.Models
{
    public class OrderConfirmationViewModel
    {
        public string OrderId { get; set; }

        [Display(Name = "Order Number")]
        public string OrderNumber { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Subtotal")]
        [DataType(DataType.Currency)]
        public decimal Subtotal { get; set; }

        [Display(Name = "Shipping Cost")]
        [DataType(DataType.Currency)]
        public decimal ShippingCost { get; set; }

        [Display(Name = "Discount Amount")]
        [DataType(DataType.Currency)]
        public decimal DiscountAmount { get; set; }

        [Display(Name = "Total")]
        [DataType(DataType.Currency)]
        public decimal Total { get; set; }

        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        [Display(Name = "Payment Status")]
        public string PaymentStatus { get; set; }

        [Display(Name = "Shipping Address")]
        public Address ShippingAddress { get; set; }

        [Display(Name = "Estimated Delivery Date")]
        public DateTime? EstimatedDeliveryDate { get; set; }

        [Display(Name = "Confirmation Number")]
        public string ConfirmationNumber { get; set; }

        [Display(Name = "Email Confirmation Sent")]
        public bool EmailConfirmationSent { get; set; }

        [Display(Name = "Customer Email")]
        [EmailAddress]
        public string CustomerEmail { get; set; }
    }
}
