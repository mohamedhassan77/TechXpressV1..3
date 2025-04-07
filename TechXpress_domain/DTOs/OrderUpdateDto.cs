using TechXpress_domain.Enums;
using System;

namespace TechXpress_domain.DTOs
{
    public class OrderUpdateDto
    {
        public int OrderId { get; set; }
        public OrderStatus NewStatus { get; set; }
 
          public DateTime? ExpectedDeliveryDate { get; set; }
    }
}
