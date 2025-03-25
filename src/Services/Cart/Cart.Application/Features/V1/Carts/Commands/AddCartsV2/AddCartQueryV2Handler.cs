using Cart.Application.Common.Interfaces;
using Cart.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Application.Features.V1.Carts.Commands.AddCartsV2
{
    public class AddCartQueryV2Handler : IRequestHandler<AddCartQueryV2, CartInformation>
    {
        private readonly ICartRepository _cartService;
        public AddCartQueryV2Handler(ICartRepository cartService)
        {
            _cartService = cartService;
        }
        public async Task<CartInformation> Handle(AddCartQueryV2 request, CancellationToken cancellationToken)
        {
            var updatedBasket = await _cartService.AddToCartV2(request.UserId, request.CourseId);
            return updatedBasket;
        }
    }
}
