using System;

namespace TechXpress_domain.DTOs
{
    public class OrderDto
    {
        public int orderId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public string UserId { get; set; }
         public int ItemsCount { get; set; }
    }
}
