using Cart.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.ComponentModel.DataAnnotations;
using Entity = Cart.Domain.Entities;
using System.Net;
using Cart.Application.Features.V1.Carts.Queries.GetCarts;
using MediatR;

namespace Cart.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : Controller
    {
        private readonly ICartRepository _basketService;
        private readonly IMediator _mediator;
        public CartController(ICartRepository basketService, IMediator mediator)
        {
            _basketService = basketService;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("{username}", Name = "GetCart")]
        [ProducesResponseType(typeof(Entity.Cart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Entity.Cart>> GetBasket([Required] string username)
        {
            var query = new GetCartQuery(username);
            var cart = await _mediator.Send(query);
            return Ok(cart );
        }

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
