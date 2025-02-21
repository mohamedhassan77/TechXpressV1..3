using System.Collections.Generic;
using System.Linq;
using TechXpress.Models;

namespace TechXpress.Models
{
    public class CartViewModel
    {
        public int CartId { get; set; }
        public string? UserId { get; set; }
        public string? SessionId { get; set; }
        public ICollection<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
        public decimal Subtotal => Items?.Sum(item => item.Product.Price * item.Quantity) ?? 0;
        public decimal ShippingCost { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Total => Subtotal + ShippingCost - DiscountAmount;
        public string? PromoCode { get; set; }
        public int ItemCount => Items?.Sum(item => item.Quantity) ?? 0;
    }

    public class CartItemViewModel
    {
        public int Id { get; set; }
        public ProductViewModel Product { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
        public decimal Subtotal => Quantity * PriceAtPurchase;
    }
}