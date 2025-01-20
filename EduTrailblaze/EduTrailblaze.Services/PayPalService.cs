using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.Extensions.Configuration;
//using PayPal.Api;

namespace EduTrailblaze.Services
{
    public class PayPalService : IPayPalService
    {
        private readonly IConfiguration _configuration;
        private readonly IRepository<EduTrailblaze.Entities.Order, int> _orderRepository;
        private readonly IPaymentService _paymentService;

        public PayPalService(IConfiguration configuration, IRepository<EduTrailblaze.Entities.Order, int> orderRepository, IPaymentService paymentService)
        {
            _configuration = configuration;
            _orderRepository = orderRepository;
            _paymentService = paymentService;
        }

        //private APIContext GetApiContext()
        //{
        //    try
        //    {
        //        var clientId = _configuration["PayPal:ClientId"];
        //        var clientSecret = _configuration["PayPal:Secret"];
        //        var config = new Dictionary<string, string>
        //    {
        //        { "mode", _configuration["PayPal:Mode"] } // "sandbox" or "live"
        //    };

        //        var accessToken = new OAuthTokenCredential(clientId, clientSecret, config).GetAccessToken();
        //        return new APIContext(accessToken) { Config = config };
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("An error occurred while getting the API context.", ex);
        //    }
        //}

       // public Payment CreatePayment(decimal amount, int orderId, int paymentId)
       // {
            //    try
            //    {
            //        var apiContext = GetApiContext();
            //        string returnUrl = _configuration["PayPal:ReturnUrl"];
            //        string cancelUrl = _configuration["PayPal:CancelUrl"];

            //        var payment = new Payment
            //        {
            //            intent = "sale",
            //            payer = new Payer { payment_method = "paypal" },
            //            transactions = new List<Transaction>
            //    {
            //        new Transaction
            //        {
            //            amount = new Amount { total = amount.ToString("F"), currency = "USD" },
            //            description = "Edu Trailblaze",
            //            custom = $"{orderId}-{paymentId}"
            //        }
            //    },
            //            redirect_urls = new RedirectUrls
            //            {
            //                return_url = returnUrl,
            //                cancel_url = cancelUrl
            //            }
            //        };

            //        return payment.Create(apiContext);
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception("An error occurred while creating the payment.", ex);
            //    }
            //}

            //public string CreatePaymentUrl(decimal amount, int orderId, int paymentId)
            //{
            //    try
            //    {
            //        var payment = CreatePayment(amount, orderId, paymentId);
            //        var approvalUrl = payment.links.FirstOrDefault(link => link.rel == "approval_url")?.href;

            //        return approvalUrl;
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception("An error occurred while creating the payment URL.", ex);
            //    }
            //}

            //public async Task<PaymentResponse> ExecutePayment(string payPalPaymentId, string payerId)
            //{
            //    try
            //    {
            //        var apiContext = GetApiContext();
            //        var paymentExecution = new PaymentExecution { payer_id = payerId };
            //        var payPalPayment = new Payment { id = payPalPaymentId };

            //        var executedPayment = payPalPayment.Execute(apiContext, paymentExecution);

            //        if (executedPayment == null || executedPayment.transactions == null || executedPayment.transactions.Count == 0)
            //        {
            //            throw new Exception("Invalid payment response received from PayPal.");
            //        }

            //        var custom = executedPayment.transactions[0].custom;
            //        var orderId = int.Parse(custom.Split('-')[0]);
            //        var paymentId = int.Parse(custom.Split('-')[1]);

            //        if (executedPayment.state == "approved")
            //        {
            //            //UpdatePaymentRequest updatePaymentRequest = new UpdatePaymentRequest
            //            //{
            //            //    PaymentId = paymentId,
            //            //    PaymentStatus = "Success",
            //            //};
            //            //await _paymentService.UpdatePayment(updatePaymentRequest);

            //            //var order = await _orderRepository.GetByIdAsync(paymentId);
            //            //if (order == null)
            //            //{
            //            //    throw new Exception("Order not found.");
            //            //}
            //            //order.OrderStatus = "Completed";
            //            //await _orderRepository.UpdateAsync(order);

            //            return new PaymentResponse
            //            {
            //                IsSuccessful = true,
            //                RedirectUrl = _configuration["FE:Url"] + $"/confirm?orderId={orderId}"
            //            };
            //        }
            //        else
            //        {
            //            // Handle payment failure
            //            //UpdatePaymentRequest updatePaymentRequest = new()
            //            //{
            //            //    PaymentId = paymentId,
            //            //    PaymentStatus = "Failed",
            //            //};

            //            //await _paymentService.UpdatePayment(updatePaymentRequest);

            //            return new PaymentResponse
            //            {
            //                IsSuccessful = false,
            //                RedirectUrl = _configuration["FE:Url"] + $"/reject?orderId={orderId}"
            //            };
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception("An error occurred while executing the payment.", ex);
            //    }
            //}
        }
    }

