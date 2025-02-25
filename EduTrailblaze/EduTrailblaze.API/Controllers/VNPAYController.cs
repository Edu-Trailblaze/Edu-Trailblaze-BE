using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduTrailblaze.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VNPAYController : ControllerBase
    {
        private readonly IVNPAYService _vnpayService;

        public VNPAYController(IVNPAYService vnpayService)
        {
            _vnpayService = vnpayService;
        }

        [HttpGet("payment-url")]
        public IActionResult GetPaymentUrl(decimal amount, int orderId, int paymentId)
        {
            try
            {
                string paymentUrl = _vnpayService.CreatePaymentUrl(amount, orderId, paymentId);
                return Ok(paymentUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("validate")]
        public async Task<IActionResult> ValidatePaymentResponse([FromQuery] string queryString)
        {
            try
            {
                var response = await _vnpayService.ValidatePaymentResponse(queryString);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
