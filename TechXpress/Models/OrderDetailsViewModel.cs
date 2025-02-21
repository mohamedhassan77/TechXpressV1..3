using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TechXpress_domain.Entities;

namespace TechXpress.Models
{
    public class OrderDetailsViewModel
    {
        [Required]
        [Display(Name = "Order ID")]
        public string OrderId { get; set; }

        [Display(Name = "Order Date")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Current Status")]
        public string CurrentStatus { get; set; }

        [Display(Name = "Order Status History")]
        public List<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();

        [Display(Name = "Order Items")]
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        [Display(Name = "Shipping Address")]
        public Address ShippingAddress { get; set; }

        [Display(Name = "Billing Address")]
        public Address BillingAddress { get; set; }

        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        [Display(Name = "Payment Details")]
        public string PaymentDetails { get; set; }

        [Display(Name = "Subtotal")]
        [DataType(DataType.Currency)]
        public decimal Subtotal { get; set; }

        [Display(Name = "Shipping Cost")]
        [DataType(DataType.Currency)]
        public decimal ShippingCost { get; set; }

        [Display(Name = "Tax")]
        [DataType(DataType.Currency)]
        public decimal Tax { get; set; }

        [Display(Name = "Discount")]
        [DataType(DataType.Currency)]
        public decimal Discount { get; set; }

        [Display(Name = "Total")]
        [DataType(DataType.Currency)]
        public decimal Total { get; set; }

        [Display(Name = "Tracking Number")]
        public string? TrackingNumber { get; set; }

        [Display(Name = "Estimated Delivery Date")]
        [DataType(DataType.Date)]
        public DateTime? EstimatedDeliveryDate { get; set; }
    }

    public class OrderStatusHistory
    {
        public DateTime StatusDate { get; set; }
        public string Status { get; set; }
        public string? Description { get; set; }
    }
}
