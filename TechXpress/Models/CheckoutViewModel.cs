using System;
using System.ComponentModel.DataAnnotations;
using TechXpress_domain.Entities;

namespace TechXpress.Models
{
    public class CheckoutViewModel
    {
        [Required]
        public string UserId { get; set; } = null!;

        public ICollection<Address> SavedAddresses { get; set; } = new List<Address>();

        public Address? NewAddress { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        [Display(Name = "Subtotal")]
        [DataType(DataType.Currency)]
        public decimal Subtotal { get; set; }

        [Display(Name = "Discount Amount")]
        [DataType(DataType.Currency)]
        public decimal DiscountAmount { get; set; }

        [Required]
        [Display(Name = "Shipping Address")]
        public Address ShippingAddress { get; set; }

        [Required]
        [Display(Name = "Billing Address")]
        public Address BillingAddress { get; set; }

        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = null!;

        [Display(Name = "Order Total")]
        [DataType(DataType.Currency)]
        public decimal OrderTotal { get; set; }

        [Display(Name = "Shipping Method")]
        public string? ShippingMethod { get; set; }

        [Display(Name = "Shipping Cost")]
        [DataType(DataType.Currency)]
        public decimal ShippingCost { get; set; }

        [Display(Name = "Tax")]
        [DataType(DataType.Currency)]
        public decimal Tax { get; set; }

        [Display(Name = "Special Instructions")]
        [StringLength(500)]
        public string? SpecialInstructions { get; set; }

        [Display(Name = "Gift Wrapping")]
        public bool IsGiftWrapping { get; set; }

        [Display(Name = "Gift Message")]
        [StringLength(200)]
        public string? GiftMessage { get; set; }

        [Display(Name = "Agree to Terms")]
        [Required(ErrorMessage = "You must agree to the terms and conditions")]
        public bool AgreeToTerms { get; set; }
    }
}