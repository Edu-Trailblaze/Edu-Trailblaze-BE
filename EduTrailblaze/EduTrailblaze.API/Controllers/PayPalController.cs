using EduTrailblaze.Services.DTOs;
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

        //[HttpGet("payment-url")]
        //public IActionResult GetPaymentUrl(decimal amount, int orderId, int paymentId)
        //{
        //    try
        //    {
        //        string paymentUrl = _payPalService.CreatePaymentUrl(amount, orderId, paymentId);
        //        return Ok(paymentUrl);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpPost("validate")]
        //public IActionResult ValidatePaymentResponse(string paymentId, string payerId)
        //{
        //    try
        //    {
               
        //         var payment = _payPalService.ExecutePayment(paymentId, payerId);
        //         Ok(payment);
        //        Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
    }
}
