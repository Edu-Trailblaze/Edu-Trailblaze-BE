using EduTrailblaze.Services.DTOs;
using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduTrailblaze.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        [HttpGet("{voucherId}")]
        public async Task<IActionResult> GetVoucher(int voucherId)
        {
            try
            {
                var voucher = await _voucherService.GetVoucher(voucherId);
                if (voucher == null)
                {
                    return NotFound();
                }
                return Ok(voucher);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("get-paging-voucher")]
        public async Task<IActionResult> GetPagingVoucher([FromQuery] GetVouchersRequest request, [FromQuery] Paging paging)
        {
            try
            {
                var voucher = await _voucherService.GetVoucherInformation(request, paging);
                if (voucher == null)
                {
                    return NotFound();
                }
                return Ok(voucher);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddVoucher([FromBody] CreateVoucherRequest voucher)
        {
            try
            {
                await _voucherService.AddVoucher(voucher);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateVoucher([FromBody] UpdateVoucherRequest voucher)
        {
            try
            {
                await _voucherService.UpdateVoucher(voucher);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
