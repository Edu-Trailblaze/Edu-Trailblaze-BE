using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Services.Helper;
using System.Web;

namespace EduTrailblaze.Services
{
    public class MoMoService : IMoMoService
    {
        private readonly IRepository<Order, int> _orderRepository;
        private readonly IPaymentService _paymentService;
        private readonly ICartService _cartService;
        private readonly MoMoSettings _momoSettings;
        private readonly IConfiguration _configuration;

        public MoMoService(IRepository<Order, int> orderRepository, IPaymentService paymentService, IConfiguration configuration, ICartService cartService)
        {
            _orderRepository = orderRepository;
            _paymentService = paymentService;
            _momoSettings = configuration.GetSection("MoMoSettings").Get<MoMoSettings>();
            _configuration = configuration;
            _cartService = cartService;
        }

        public async Task<string> CreatePaymentUrl(decimal amount, int orderId, int paymentId)
        {
            try
            {
                string requestId = Guid.NewGuid().ToString(); // Unique request ID
                string extraData = $"PaymentId:{paymentId}";
                string orderInfo = "EduTrailblaze";
                string requestType = _momoSettings.MoMoRequestType;
                string customOrderId = "O-" + orderId.ToString() + "-" + Guid.NewGuid().ToString();

                // Ensure the amount is formatted correctly
                long formattedAmount = (long)(amount * 1000); // Convert to VND

                // Create raw signature string
                string rawHash = $"accessKey={_momoSettings.MoMoAccessKey}&amount={formattedAmount}&extraData={extraData}&ipnUrl={_momoSettings.MoMoNotifyUrl}&orderId={customOrderId}&orderInfo={orderInfo}&partnerCode={_momoSettings.MoMoPartnerCode}&redirectUrl={_momoSettings.MoMoReturnUrl}&requestId={requestId}&requestType={requestType}";
                string signature = HashUtil.HmacSHA256(_momoSettings.MoMoSecretKey, rawHash);

                // Build request payload
                var paymentRequest = new
                {
                    partnerCode = _momoSettings.MoMoPartnerCode,
                    partnerName = _momoSettings.MoMoPartnerName,
                    storeId = _momoSettings.MoMoStoreId,
                    requestId,
                    amount = formattedAmount.ToString(),
                    orderId = customOrderId,
                    orderInfo,
                    redirectUrl = _momoSettings.MoMoReturnUrl,
                    ipnUrl = _momoSettings.MoMoNotifyUrl,
                    extraData,
                    requestType,
                    signature,
                    lang = _momoSettings.MoMoLang,
                };

                // Send POST request to MoMo API
                string response = await HashUtil.SendHttpRequest(_momoSettings.MoMoEndpoint, paymentRequest);

                // Parse response and extract payment URL
                var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
                if (jsonResponse.ContainsKey("payUrl"))
                {
                    return jsonResponse["payUrl"].ToString();
                }
                else
                {
                    throw new Exception($"Error creating payment URL: {jsonResponse["message"]}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error processing payment: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the payment URL: " + ex.Message);
            }
        }

        public async Task<PaymentResponse> ValidatePaymentResponse(string queryString)
        {
            try
            {
                var queryParameters = HttpUtility.ParseQueryString(queryString);
                string requestId = queryParameters["requestId"];
                int orderId = int.Parse(queryParameters["orderId"].Split('-')[1]);
                string amount = queryParameters["amount"];
                string resultCode = queryParameters["resultCode"];
                string signature = queryParameters["signature"];

                // Extract PaymentId from extraData
                string extraData = queryParameters["extraData"];
                int paymentId = int.Parse(extraData.Split(':')[1]);

                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null || order.OrderStatus != "Processing")
                {
                    return new PaymentResponse { IsSuccessful = false, RedirectUrl = _configuration["FE:Url"] + "/student/payment/paymentFailed" };
                }

                if (resultCode == "0") // MoMo Success Result Code
                {
                    UpdatePaymentRequest updatePaymentRequest = new()
                    {
                        PaymentId = paymentId,
                        PaymentStatus = "Success",
                    };

                    await _paymentService.UpdatePayment(updatePaymentRequest);

                    order.OrderStatus = "Completed";

                    await _cartService.ClearCart(order.UserId);

                    await _orderRepository.UpdateAsync(order);

                    return new PaymentResponse
                    {
                        IsSuccessful = true,
                        //RedirectUrl = _configuration["FE:Url"] + $"/payment/paymentSuccess?orderId={orderId}"
                        RedirectUrl = _configuration["FE:Url"] + $"/student/payment/paymentSuccess?paymentId={paymentId}"
                    };
                }
                else
                {
                    UpdatePaymentRequest updatePaymentRequest = new()
                    {
                        PaymentId = paymentId,
                        PaymentStatus = "Failed",
                    };

                    await _paymentService.UpdatePayment(updatePaymentRequest);

                    return new PaymentResponse
                    {
                        IsSuccessful = false,
                        //RedirectUrl = _configuration["FE:Url"] + $"/payment/paymentFailed?orderId={orderId}"
                        RedirectUrl = _configuration["FE:Url"] + $"/student/payment/paymentFailed?paymentId={paymentId}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new PaymentResponse { IsSuccessful = false, RedirectUrl = _configuration["FE:Url"] + "/reject" };
            }
        }
    }
}
