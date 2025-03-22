using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TechXpress_domain.Interfaces.Services;

namespace TechXpress_application.Services
{
    public class FakePaymentService : IPaymentService
    {
        private readonly ILogger<FakePaymentService> _logger;

        public FakePaymentService(ILogger<FakePaymentService> logger)
        {
            _logger = logger;
        }

        public Task<string> ProcessStripePaymentAsync(string userId, decimal amount, string currency, string stripeToken)
        {
            // Log and return a fake successful response
            _logger.LogInformation($"[FakePaymentService] Processing Stripe payment for user {userId}: {amount} {currency}");
            // For testing, simply return a dummy charge id
            return Task.FromResult("ch_test_stripe_charge_id");
        }

        public Task<string> ProcessPayPalPaymentAsync(string userId, decimal amount, string currency)
        {
            // Log and return a fake successful response
            _logger.LogInformation($"[FakePaymentService] Processing PayPal payment for user {userId}: {amount} {currency}");
            // For testing, simply return a dummy order id
            return Task.FromResult("order_test_paypal_order_id");
        }
    }
}
