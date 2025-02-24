using Cart.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.ComponentModel.DataAnnotations;
using Entity = Cart.Domain.Entities;
using System.Net;

namespace Cart.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : Controller
    {
        private readonly ICartRepository _basketService;
        public CartController(ICartRepository basketService)
        {
            _basketService = basketService;
        }

        //[HttpGet("{username}", Name = "GetCart")]
        //[ProducesResponseType(typeof(Entity.Cart), (int)HttpStatusCode.OK)]
        //public async Task<ActionResult<Entity.Cart>> GetBasket([Required] string username)
        //{
        //    var basket = await _basketService.GetBasket(username);
        //    return Ok(basket ?? new Entity.Cart());
        //}

        //[HttpPost(Name = "AddBasket")]
        //[ProducesResponseType(typeof(Entity.Cart), (int)HttpStatusCode.OK)]
        //public async Task<ActionResult<Entity.Cart>> UpdateBasket([FromBody] Entity.Cart basket)
        //{
        //    var option = new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTime.UtcNow.AddMinutes(1)).SetSlidingExpiration(TimeSpan.FromMinutes(10));
        //    var updatedBasket = await _basketService.UpdateCart(basket, option);
        //    return Ok(updatedBasket);
        //}
        //[HttpDelete("{username}", Name = "DeleteCart")]
        //[ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        //public async Task<ActionResult<bool>> DeleteBasket([Required] string username)
        //{
        //    return Ok(await _basketService.DeleteBasket(username));
        //}
    }
}
