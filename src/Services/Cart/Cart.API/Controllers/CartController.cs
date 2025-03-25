using Cart.Application.Features.V1.Carts.Commands.AddCarts;
using Cart.Application.Features.V1.Carts.Commands.AddCartsV2;
using Cart.Application.Features.V1.Carts.Commands.DeleteCarts;
using Cart.Application.Features.V1.Carts.Queries.GetCarts;
using Cart.Application.Features.V1.Carts.Queries.ViewCarts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Entity = Cart.Domain.Entities;

namespace Cart.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : Controller
    {
        private readonly IMediator _mediator;
        
        public CartController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            
        }

        [HttpGet("get-cart-by-userId", Name = "GetCart")]
        [ProducesResponseType(typeof(Entity.Cart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Entity.Cart>> GetCart([Required] string userId)
        {
            var query = new GetCartQuery(userId);
            var cart = await _mediator.Send(query);
            return Ok(cart);
        }
        [HttpGet("view-cart")]
        [ProducesResponseType(typeof(Entity.Cart), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ViewCart(string? userId)
        {
            var query = new ViewCartQuery(userId);
            var cart = await _mediator.Send(query);
            return Ok(cart);

        }
            [HttpPost(Name = "AddToCart")]
        [ProducesResponseType(typeof(Entity.Cart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Entity.Cart>> UpdateCart([FromBody] Entity.Cart cart)
        {
            var updatedCart = await _mediator.Send(new AddCartQuery(cart));
            return Ok(updatedCart);
        }
        [HttpDelete("{username}", Name = "DeleteCart")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> DeleteCart([Required] string username)
        {
            return Ok(await _mediator.Send(new DeleteCartQuery(username)));
        }
        [HttpPost(Name = "add-item-to-cart-v2")]
        [ProducesResponseType(typeof(Entity.Cart), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> AddToCartV2(string? userId, int courseId)
        {
            
                var cartItems = await _mediator.Send(new AddCartQueryV2(userId,courseId));
                return Ok(cartItems);
            
            
        }

    }
}
