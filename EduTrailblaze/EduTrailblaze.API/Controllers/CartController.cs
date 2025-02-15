using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduTrailblaze.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        //[HttpGet("get-system-cart")]
        //public async Task<IActionResult> GetCart(string userId)
        //{
        //    try
        //    {
        //        var cart = await _cartService.GetSystemCart(userId);
        //        if (cart == null) return NotFound();
        //        return Ok(cart);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpPost("add-item-to-system-cart")]
        //public async Task<IActionResult> AddItemToCart(int courseId, string userId)
        //{
        //    try
        //    {
        //        await _cartService.AddItemToSystemCart(courseId, userId);
        //        return Ok(new { message = "Successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpDelete("remove-item-from-system-cart")]
        //public async Task<IActionResult> RemoveItemFromCart(int courseId, string userId)
        //{
        //    try
        //    {
        //        await _cartService.RemoveItemFromSystemCart(courseId, userId);
        //        return Ok(new { message = "Successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpGet("number-of-items-in-system-cart")]
        //public async Task<IActionResult> NumberOfItemInSystemCart(string userId)
        //{
        //    try
        //    {
        //        var numberOfItems = await _cartService.NumberOfItemInSystemCart(userId);
        //        return Ok(numberOfItems);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpDelete("delete-cookie-cart")]
        //public IActionResult DeleteCartInCookie(string? userId)
        //{
        //    try
        //    {
        //        _cartService.DeleteCartInCookie(userId);
        //        return Ok(new { message = "Successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpGet("number-of-items-in-cookie-cart")]
        //public IActionResult NumberOfItemsInCookieCart(string? userId)
        //{
        //    try
        //    {
        //        var numberOfItems = _cartService.NumberOfItemsInCookieCart(userId);
        //        return Ok(numberOfItems);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpGet("get-cookie-cart")]
        //public IActionResult GetCookieCart(string? userId)
        //{
        //    try
        //    {
        //        var cart = _cartService.GetCookieCart(userId);
        //        return Ok(cart);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpPost("save-item-to-cookie-cart")]
        //public async Task<IActionResult> SaveCartToCookie(int courseId, string? userId)
        //{
        //    try
        //    {
        //        await _cartService.SaveCartToCookie(courseId, userId);
        //        return Ok(new { message = "Successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpDelete("remove-item-from-cookie-cart")]
        //public IActionResult RemoveCourseFromCart(int courseId, string? userId)
        //{
        //    try
        //    {
        //        _cartService.RemoveCourseFromCookieCart(courseId, userId);
        //        return Ok(new { message = "Successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpGet("get-cart-by-userId")]
        public async Task<IActionResult> GetCart(string? userId)
        {
            try
            {
                var numberOfItems = await _cartService.GetCart(userId);
                return Ok(numberOfItems);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("number-of-items-in-cart")]
        public async Task<IActionResult> NumberOfItemsInCart(string? userId)
        {
            try
            {
                var numberOfItems = await _cartService.NumberOfItemsInCart(userId);
                return Ok(numberOfItems);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("view-cart")]
        public async Task<IActionResult> ViewCart(string? userId)
        {
            try
            {
                var cart = await _cartService.ViewCart(userId);
                if (cart == null) return NotFound();
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("add-item-to-cart")]
        public async Task<IActionResult> AddToCart(string? userId, int courseId)
        {
            try
            {
                await _cartService.AddToCart(userId, courseId);
                return Ok(new { message = "Successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost("add-item-to-cart-v2")]
        public async Task<IActionResult> AddToCartV2(string? userId, int courseId)
        {
            try
            {
                var cartItems = await _cartService.AddToCartV2(userId, courseId);
                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("remove-item-from-cart")]
        public async Task<IActionResult> RemoveFromCart(string? userId, int courseId)
        {
            try
            {
                await _cartService.RemoveFromCart(userId, courseId);
                return Ok(new { message = "Successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("clear-cart")]
        public async Task<IActionResult> ClearCart(string? userId)
        {
            try
            {
                await _cartService.ClearCart(userId);
                return Ok(new { message = "Successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
