//using System;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using Stripe;
//using PayPalCheckoutSdk.Core;
//using PayPalCheckoutSdk.Orders;
//using PayPalHttp;
//using TechXpress_domain.Interfaces.Services;

//namespace TechXpress_application.Services
//{
//    public class PaymentService : IPaymentService
//    {
//        private readonly IConfiguration _configuration;
//        private readonly ILogger<PaymentService> _logger;

//        public PaymentService(IConfiguration configuration, ILogger<PaymentService> logger)
//        {
//            _configuration = configuration;
//            _logger = logger;
//        }

//        //  Stripe Payment Processing
//        public async Task<string> ProcessStripePaymentAsync(string userId, decimal amount, string currency, string stripeToken)
//        {
//            try
//            {
//                StripeConfiguration.ApiKey = _configuration["PaymentSettings:StripeSecretKey"];

//                var options = new ChargeCreateOptions
//                {
//                    Amount = (long)(amount * 100), // Convert to smallest currency unit
//                    Currency = currency,
//                    Description = $"TechXpress Order Payment - User {userId}",
//                    Source = stripeToken
//                };

//                var service = new ChargeService();
//                Charge charge = await service.CreateAsync(options);

//                return charge.Status == "succeeded" ? charge.Id : "Payment failed.";
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError($"Stripe Payment Error: {ex.Message}");
//                return $"Error: {ex.Message}";
//            }
//        }



//        public async Task<string> ProcessPayPalPaymentAsync(string userId, decimal amount, string currency)
//        {
//            try
//            {
//                PayPalEnvironment environment;

//                if (_configuration["PaymentSettings:PayPalMode"] == "sandbox")
//                {
//                    environment = new SandboxEnvironment(
//                        _configuration["PaymentSettings:PayPalClientId"],
//                        _configuration["PaymentSettings:PayPalClientSecret"]
//                    );
//                }
//                else
//                {
//                    environment = new LiveEnvironment(
//                        _configuration["PaymentSettings:PayPalClientId"],
//                        _configuration["PaymentSettings:PayPalClientSecret"]
//                    );
//                }

//                var client = new PayPalHttpClient(environment);

//                var orderRequest = new OrderRequest
//                {
//                    CheckoutPaymentIntent = "CAPTURE",
//                    PurchaseUnits = new List<PurchaseUnitRequest> 
//            {
//                new PurchaseUnitRequest
//                {
//                    AmountWithBreakdown = new AmountWithBreakdown
//                    {
//                        CurrencyCode = currency,
//                        Value = amount.ToString("F2")
//                    }
//                }
//            }
//                };

//                var request = new OrdersCreateRequest();
//                request.Prefer("return=representation");
//                request.RequestBody(orderRequest);

//                var response = await client.Execute(request);
//                var result = response.Result<Order>();

//                return result.Status == "CREATED" ? result.Id : "Payment failed.";
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError($"PayPal Payment Error: {ex.Message}");
//                return $"Error: {ex.Message}";
//            }
//        }

//    }
//}
