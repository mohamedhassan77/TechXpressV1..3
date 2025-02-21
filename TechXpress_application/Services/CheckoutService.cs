using System;
using System.Linq;
using System.Threading.Tasks;
using TechXpress_domain.Entities;
using TechXpress_domain.Interfaces.Repositories;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress_application.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPaymentService _paymentService;
        private readonly IEmailService _emailService;
        private readonly IUserProfileService _userService;

        public CheckoutService(
            ICartRepository cartRepository,
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IPaymentService paymentService,
            IEmailService emailService,
            IUserProfileService userService)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _paymentService = paymentService;
            _emailService = emailService;
            _userService = userService;
        }

        public async Task<string> ProcessCheckoutAsync(string userId, string paymentMethod, string transactionId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
                return "Error: Your cart is empty.";

            decimal totalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price);
            string transaction = string.Empty;

            if (paymentMethod.ToLower() == "stripe")
            {
                transaction = await _paymentService.ProcessStripePaymentAsync(userId, totalAmount, "USD", transactionId);
            }
            else if (paymentMethod.ToLower() == "paypal")
            {
                transaction = await _paymentService.ProcessPayPalPaymentAsync(userId, totalAmount, "USD");
            }
            else
            {
                return "Error: Invalid payment method.";
            }

            if (string.IsNullOrEmpty(transaction) || transaction.Contains("Error"))
                return "Error: Payment failed. " + transaction;

            var order = new Order
            {
                OrderNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalPrice = totalAmount,
                Discount = 0,
                PaymentMethod = paymentMethod,
                TransactionId = transaction,
                Status = "Paid",
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    PriceAtPurchase = ci.Product.Price
                }).ToList()
            };

            await _orderRepository.AddOrderAsync(order);
            await _orderRepository.SaveChangesAsync();
            await _cartRepository.ClearCartAsync(userId);
            await _cartRepository.SaveChangesAsync();

            string userEmail = await _userService.GetUserEmailByIdAsync(userId);
            if (!string.IsNullOrEmpty(userEmail))
            {
                string emailBody = $@"
                    <h2>Order Confirmation</h2>
                    <p>Your order #{order.OrderNumber} has been placed successfully.</p>
                    <p>Transaction ID: {transaction}</p>
                    <p>Total Amount: ${totalAmount:F2}</p>
                    <p>Thank you for shopping with TechXpress!</p>";
                await _emailService.SendEmailAsync(userEmail, "Order Confirmation - TechXpress", emailBody);
            }

            return "Order placed successfully. Email receipt sent.";
        }
    }
}
