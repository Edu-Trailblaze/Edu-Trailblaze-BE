using EduTrailblaze.Services;
using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduTrailblaze.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoMoController : ControllerBase
    {
        private readonly IMoMoService _momoService;

        public MoMoController(IMoMoService momoService)
        {
            _momoService = momoService;
        }

        [HttpGet("payment-url")]
        public async Task<IActionResult> GetPaymentUrl(decimal amount, int orderId, int paymentId)
        {
            try
            {
                string paymentUrl = await _momoService.CreatePaymentUrl(amount, orderId, paymentId);
                return Redirect(paymentUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("validate")]
        public async Task<IActionResult> ValidatePaymentResponse()
        {
            try
            {
                if (Request.QueryString.HasValue)
                {
                    string queryString = Request.QueryString.Value;

                    var response = await _momoService.ValidatePaymentResponse(queryString);
                    return Redirect(response.RedirectUrl);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
