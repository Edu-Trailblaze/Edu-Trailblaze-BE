using EduTrailblaze.Entities;
using EduTrailblaze.Repositories.Interfaces;
using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Services.Helper;
using System.Net;
using System.Web;

namespace EduTrailblaze.Services
{
    public class VNPAYService : IVNPAYService
    {
        private readonly IRepository<Order, int> _orderRepository;
        private readonly IPaymentService _paymentService;
        private readonly ICartService _cartService;
        private readonly VNPAYSettings _VNPAYSettings;
        private readonly IConfiguration _configuration;

        public VNPAYService(IRepository<Order, int> orderRepository, IPaymentService paymentService, IConfiguration configuration, ICartService cartService)
        {
            _configuration = configuration;
            _orderRepository = orderRepository;
            _paymentService = paymentService;
            _VNPAYSettings = configuration.GetSection("VNPAYSettings").Get<VNPAYSettings>();
            _cartService = cartService;
        }

        public string CreatePaymentUrl(decimal amount, int orderId, int paymentId)
        {
            try
            {
                string hostName = Dns.GetHostName();
                string clientIPAddress = Dns.GetHostAddresses(hostName).GetValue(0).ToString();
                PayLib pay = new PayLib();
                var vnp_Amount = amount * 100000;
                pay.AddRequestData("vnp_Version", PayLib.VERSION);
                pay.AddRequestData("vnp_Command", "pay");
                pay.AddRequestData("vnp_TmnCode", _VNPAYSettings.VnPayTmnCode);
                pay.AddRequestData("vnp_Amount", vnp_Amount.ToString("0"));
                pay.AddRequestData("vnp_BankCode", "");
                pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                pay.AddRequestData("vnp_CurrCode", "VND");
                pay.AddRequestData("vnp_IpAddr", clientIPAddress);
                pay.AddRequestData("vnp_Locale", "vn");
                pay.AddRequestData("vnp_OrderInfo", $"PaymentId:{paymentId}");
                pay.AddRequestData("vnp_OrderType", "other");
                pay.AddRequestData("vnp_ReturnUrl", _VNPAYSettings.VnPayReturnUrl);
                pay.AddRequestData("vnp_TxnRef", "O-" + orderId.ToString() + "-" + Guid.NewGuid().ToString()); // Use OrderId as vnp_TxnRef

                string paymentUrl = pay.CreateRequestUrl(_VNPAYSettings.VnPayUrl, _VNPAYSettings.VnPayHashSecret);
                return paymentUrl;
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
                var json = HttpUtility.ParseQueryString(queryString);
                var orderId = int.Parse(json["vnp_TxnRef"].Split('-')[1]);

                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    return new PaymentResponse
                    {
                        IsSuccessful = false,
                        RedirectUrl = _configuration["FE:Url"] + "/payment/paymentFailed"
                    };
                }

                // Extract PaymentId from vnp_OrderInfo
                var orderInfo = json["vnp_OrderInfo"];
                var paymentId = int.Parse(orderInfo.Split(':')[1]);

                string vnp_ResponseCode = json["vnp_ResponseCode"].ToString();
                string vnp_SecureHash = json["vnp_SecureHash"].ToString();
                var pos = queryString.IndexOf("&vnp_SecureHash");
                bool checkSignature = ValidateSignature(queryString.Substring(1, pos - 1), vnp_SecureHash, _VNPAYSettings.VnPayHashSecret);

                if (order.OrderStatus != "Pending" && order != null)
                {
                    return new PaymentResponse
                    {
                        IsSuccessful = false,
                        RedirectUrl = _configuration["FE:Url"] + "/payment/paymentFailed"
                    };
                }

                if (checkSignature && _VNPAYSettings.VnPayTmnCode == json["vnp_TmnCode"].ToString())
                {
                    if (vnp_ResponseCode == "00" && json["vnp_TransactionStatus"] == "00")
                    {
                        UpdatePaymentRequest updatePaymentRequest = new UpdatePaymentRequest
                        {
                            PaymentId = paymentId,
                            PaymentStatus = "Success",
                        };
                        await _paymentService.UpdatePayment(updatePaymentRequest);

                        order.OrderStatus = "Completed";
                        await _orderRepository.UpdateAsync(order);

                        await _cartService.ClearCart(order.UserId);

                        return new PaymentResponse
                        {
                            IsSuccessful = true,
                            //RedirectUrl = _configuration["FE:Url"] + $"/payment/paymentSuccess?orderId={json["vnp_TxnRef"].ToString()}"
                            RedirectUrl = _configuration["FE:Url"] + $"/payment/paymentSuccess?paymentId={paymentId}"
                        };
                    }
                    else
                    {
                        if (json["vnp_BankTranNo"]?.ToString() != null || json["vnp_TxnRef"]?.ToString() != null)
                        {
                            UpdatePaymentRequest updatePaymentRequest = new()
                            {
                                PaymentId = paymentId,
                                PaymentStatus = "Failed",
                            };

                            await _paymentService.UpdatePayment(updatePaymentRequest);
                        }
                        return new PaymentResponse
                        {
                            IsSuccessful = false,
                            //RedirectUrl = _configuration["FE:Url"] + $"/payment/paymentFailed?orderId={json["vnp_TxnRef"].ToString()}"
                            RedirectUrl = _configuration["FE:Url"] + $"/payment/paymentFailed?paymentId={paymentId}"
                        };
                    }
                }
                else
                {
                    return new PaymentResponse
                    {
                        IsSuccessful = false,
                        RedirectUrl = _configuration["FE:Url"] + "/payment/paymentFailed"
                    };
                }
            }
            catch (Exception ex)
            {
                return new PaymentResponse
                {
                    IsSuccessful = false,
                    RedirectUrl = _configuration["FE:Url"] + "/payment/paymentFailed"
                };
            }
        }

        private bool ValidateSignature(string rspraw, string inputHash, string secretKey)
        {
            string myChecksum = Utils.HmacSHA512(secretKey, rspraw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
