using EduTrailblaze.Services.DTOs;

namespace EduTrailblaze.Services.Interfaces
{
    public interface IPayPalService
    {
        //Payment CreatePayment(decimal amount, int orderId, int paymentId);

        //string CreatePaymentUrl(decimal amount, int orderId, int paymentId);

        Task<string> CreatePaymentUrl(decimal amount, int orderId, int paymentId);

        Task<PaymentResponse> ExecutePayment(string payPalPaymentId, string token, string payerId);
    }
}
