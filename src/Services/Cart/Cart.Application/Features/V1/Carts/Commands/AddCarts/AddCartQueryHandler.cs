using Cart.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Cart.Application.Features.V1.Carts.Commands.AddCarts
{
    public class AddCartQueryHandler : IRequestHandler<AddCartQuery, Cart.Domain.Entities.Cart>
    {
        private readonly ICartRepository _cartService;
        public AddCartQueryHandler(ICartRepository cartService)
        {
            _cartService = cartService;
        }
        public async Task<Domain.Entities.Cart> Handle(AddCartQuery request, CancellationToken cancellationToken)
        {
            var option = new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTime.UtcNow.AddMinutes(1)).SetSlidingExpiration(TimeSpan.FromMinutes(10));
            var updatedBasket = await _cartService.UpdateCart(request.Cart, option);
            return updatedBasket;
        }
    }
}
