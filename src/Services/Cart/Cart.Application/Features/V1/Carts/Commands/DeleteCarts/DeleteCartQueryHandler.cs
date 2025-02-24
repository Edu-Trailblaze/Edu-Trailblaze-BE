using Cart.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Application.Features.V1.Carts.Commands.DeleteCarts
{
    public record DeleteCartQuery(string Username) : IRequest<bool>;
    internal class DeleteCartQueryHandler : IRequestHandler<DeleteCartQuery, bool>
    {
        private readonly ICartRepository _cartService;
        public DeleteCartQueryHandler(ICartRepository cartService)
        {
            _cartService = cartService;
        }
        public async Task<bool> Handle(DeleteCartQuery request, CancellationToken cancellationToken)
        {
             return await _cartService.DeleteCartByUsername(request.Username);
        }
    }
}
