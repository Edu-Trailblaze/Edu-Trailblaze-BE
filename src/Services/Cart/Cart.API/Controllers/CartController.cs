using Cart.Application.Features.V1.Carts.Commands.AddCarts;
using Cart.Application.Features.V1.Carts.Commands.DeleteCarts;
using Cart.Application.Features.V1.Carts.Queries.GetCarts;
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

        [HttpGet("{username}", Name = "GetCart")]
        [ProducesResponseType(typeof(Entity.Cart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Entity.Cart>> GetCart([Required] string username)
        {
            var query = new GetCartQuery(username);
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
    }
}
