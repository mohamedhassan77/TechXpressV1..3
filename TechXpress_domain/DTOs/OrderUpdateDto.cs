using TechXpress_domain.Enums;
using System;

namespace TechXpress_domain.DTOs
{
    public class OrderUpdateDto
    {
        public OrderStatus Status { get; set; }

          public DateTime? ExpectedDeliveryDate { get; set; }
    }
}
