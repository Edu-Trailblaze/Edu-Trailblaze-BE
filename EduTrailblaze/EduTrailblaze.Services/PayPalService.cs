using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace EduTrailblaze.Services
{
    public class PayPalService : IPayPalService
    {
        private readonly IConfiguration _configuration;
        private readonly IRepository<Order, int> _orderRepository;
        private readonly IPaymentService _paymentService;
        private readonly HttpClient _httpClient;

        public PayPalService(IConfiguration configuration, IRepository<Order, int> orderRepository, IPaymentService paymentService, HttpClient httpClient)
        {
            _configuration = configuration;
            _orderRepository = orderRepository;
            _paymentService = paymentService;
            _httpClient = httpClient;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            try
            {
                var clientId = _configuration["PayPal:ClientId"];
                var clientSecret = _configuration["PayPal:Secret"];

                var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                });

                var response = await _httpClient.PostAsync("https://api-m.sandbox.paypal.com/v1/oauth2/token", content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to retrieve PayPal access token.");
                }

                var responseData = await response.Content.ReadAsStringAsync();
                dynamic tokenResponse = JsonConvert.DeserializeObject(responseData);
                return tokenResponse.access_token;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while obtaining the PayPal access token.", ex);
            }
        }

        public async Task<string> CreatePaymentUrl(decimal amount, int orderId, int paymentId)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var paymentRequest = new
                {
                    intent = "sale",
                    payer = new { payment_method = "paypal" },
                    transactions = new[]
                    {
                        new
                        {
                            amount = new { total = amount.ToString("F"), currency = "USD" },
                            description = "Edu Trailblaze",
                            custom = $"{orderId}-{paymentId}"
                        }
                    },
                    redirect_urls = new
                    {
                        return_url = _configuration["PayPal:ReturnUrl"],
                        cancel_url = _configuration["PayPal:CancelUrl"]
                    }
                };

                var content = new StringContent(JsonConvert.SerializeObject(paymentRequest), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://api-m.sandbox.paypal.com/v1/payments/payment", content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to create PayPal payment.");
                }

                var responseData = await response.Content.ReadAsStringAsync();
                dynamic paymentResponse = JsonConvert.DeserializeObject(responseData);

                var approvalUrl = ((IEnumerable<dynamic>)paymentResponse.links).FirstOrDefault(link => link.rel == "approval_url")?.href;

                return approvalUrl;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the payment.", ex);
            }
        }

        public async Task<PaymentResponse> ExecutePayment(string payPalPaymentId, string token, string payerId)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var paymentExecution = new { payer_id = payerId };

                var content = new StringContent(JsonConvert.SerializeObject(paymentExecution), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"https://api-m.sandbox.paypal.com/v1/payments/payment/{payPalPaymentId}/execute", content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to execute PayPal payment.");
                }

                var responseData = await response.Content.ReadAsStringAsync();
                dynamic executedPayment = JsonConvert.DeserializeObject(responseData);

                var custom = (string)executedPayment.transactions[0].custom;
                var orderId = int.Parse(custom.Split('-')[0]);
                var paymentId = int.Parse(custom.Split('-')[1]);

                if (executedPayment.state == "approved")
                {
                    //UpdatePaymentRequest updatePaymentRequest = new UpdatePaymentRequest
                    //{
                    //    PaymentId = paymentId,
                    //    PaymentStatus = "Success",
                    //};
                    //await _paymentService.UpdatePayment(updatePaymentRequest);

                    //var order = await _orderRepository.GetByIdAsync(paymentId);
                    //if (order == null)
                    //{
                    //    throw new Exception("Order not found.");
                    //}
                    //order.OrderStatus = "Completed";
                    //await _orderRepository.UpdateAsync(order);

                    return new PaymentResponse
                    {
                        IsSuccessful = true,
                        RedirectUrl = _configuration["FE:Url"] + $"/payment/paymentSuccess?orderId={orderId}"
                    };
                }
                else
                {
                    // Handle payment failure
                    //UpdatePaymentRequest updatePaymentRequest = new()
                    //{
                    //    PaymentId = paymentId,
                    //    PaymentStatus = "Failed",
                    //};

                    //await _paymentService.UpdatePayment(updatePaymentRequest);

                    return new PaymentResponse
                    {
                        IsSuccessful = false,
                        RedirectUrl = _configuration["FE:Url"] + $"/payment/paymentFailed?orderId={orderId}"
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while executing the payment.", ex);
            }
        }
    }
}


//using EduTrailblaze.Repositories.Interfaces;
//using EduTrailblaze.Services.DTOs;
//using EduTrailblaze.Services.Interfaces;
//using Microsoft.Extensions.Configuration;
//using PayPal.Api;

//namespace EduTrailblaze.Services
//{
//    public class PayPalService : IPayPalService
//    {
//        private readonly IConfiguration _configuration;
//        private readonly IRepository<Order, int> _orderRepository;
//        private readonly IPaymentService _paymentService;

//        public PayPalService(IConfiguration configuration, IRepository<Order, int> orderRepository, IPaymentService paymentService)
//        {
//            _configuration = configuration;
//            _orderRepository = orderRepository;
//            _paymentService = paymentService;
//        }

//        private APIContext GetApiContext()
//        {
//            try
//            {
//                var clientId = _configuration["PayPal:ClientId"];
//                var clientSecret = _configuration["PayPal:Secret"];
//                var config = new Dictionary<string, string>
//            {
//                { "mode", _configuration["PayPal:Mode"] } // "sandbox" or "live"
//            };

//                var accessToken = new OAuthTokenCredential(clientId, clientSecret, config).GetAccessToken();
//                return new APIContext(accessToken) { Config = config };
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("An error occurred while getting the API context.", ex);
//            }
//        }

//        public Payment CreatePayment(decimal amount, int orderId, int paymentId)
//        {
//            try
//            {
//                var apiContext = GetApiContext();
//                string returnUrl = _configuration["PayPal:ReturnUrl"];
//                string cancelUrl = _configuration["PayPal:CancelUrl"];

//                var payment = new Payment
//                {
//                    intent = "sale",
//                    payer = new Payer { payment_method = "paypal" },
//                    transactions = new List<Transaction>
//            {
//                new Transaction
//                {
//                    amount = new Amount { total = amount.ToString("F"), currency = "USD" },
//                    description = "Edu Trailblaze",
//                    custom = $"{orderId}-{paymentId}"
//                }
//            },
//                    redirect_urls = new RedirectUrls
//                    {
//                        return_url = returnUrl,
//                        cancel_url = cancelUrl
//                    }
//                };

//                return payment.Create(apiContext);
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("An error occurred while creating the payment.", ex);
//            }
//        }

//        public string CreatePaymentUrl(decimal amount, int orderId, int paymentId)
//        {
//            try
//            {
//                var payment = CreatePayment(amount, orderId, paymentId);
//                var approvalUrl = payment.links.FirstOrDefault(link => link.rel == "approval_url")?.href;

//                return approvalUrl;
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("An error occurred while creating the payment URL.", ex);
//            }
//        }

//        public async Task<PaymentResponse> ExecutePayment(string payPalPaymentId, string payerId)
//        {
//            try
//            {
//                var apiContext = GetApiContext();
//                var paymentExecution = new PaymentExecution { payer_id = payerId };
//                var payPalPayment = new Payment { id = payPalPaymentId };

//                var executedPayment = payPalPayment.Execute(apiContext, paymentExecution);

//                if (executedPayment == null || executedPayment.transactions == null || executedPayment.transactions.Count == 0)
//                {
//                    throw new Exception("Invalid payment response received from PayPal.");
//                }

//                var custom = executedPayment.transactions[0].custom;
//                var orderId = int.Parse(custom.Split('-')[0]);
//                var paymentId = int.Parse(custom.Split('-')[1]);

//                if (executedPayment.state == "approved")
//                {
//                    //UpdatePaymentRequest updatePaymentRequest = new UpdatePaymentRequest
//                    //{
//                    //    PaymentId = paymentId,
//                    //    PaymentStatus = "Success",
//                    //};
//                    //await _paymentService.UpdatePayment(updatePaymentRequest);

//                    //var order = await _orderRepository.GetByIdAsync(paymentId);
//                    //if (order == null)
//                    //{
//                    //    throw new Exception("Order not found.");
//                    //}
//                    //order.OrderStatus = "Completed";
//                    //await _orderRepository.UpdateAsync(order);

//                    return new PaymentResponse
//                    {
//                        IsSuccessful = true,
//                        RedirectUrl = _configuration["FE:Url"] + $"/payment/paymentSuccess?orderId={orderId}"
//                    };
//                }
//                else
//                {
//                    // Handle payment failure
//                    //UpdatePaymentRequest updatePaymentRequest = new()
//                    //{
//                    //    PaymentId = paymentId,
//                    //    PaymentStatus = "Failed",
//                    //};

//                    //await _paymentService.UpdatePayment(updatePaymentRequest);

//                    return new PaymentResponse
//                    {
//                        IsSuccessful = false,
//                        RedirectUrl = _configuration["FE:Url"] + $"/payment/paymentFailed?orderId={orderId}"
//                    };
//                }
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("An error occurred while executing the payment.", ex);
//            }
//        }
//    }
//}

