using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduTrailblaze.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayPalController : ControllerBase
    {
        private readonly IPayPalService _payPalService;

        public PayPalController(IPayPalService payPalService)
        {
            _payPalService = payPalService;
        }

        [HttpGet("payment-url")]
        public async Task<IActionResult> GetPaymentUrl(decimal amount, int orderId, int paymentId)
        {
            try
            {
                string paymentUrl = await _payPalService.CreatePaymentUrl(amount, orderId, paymentId);
                return Redirect(paymentUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("validate")]
        public async Task<IActionResult> ValidatePaymentResponse(string paymentId, string token, string payerId)
        {
            try
            {
                var payment = await _payPalService.ExecutePayment(paymentId, token, payerId);
                return Redirect(payment.RedirectUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
