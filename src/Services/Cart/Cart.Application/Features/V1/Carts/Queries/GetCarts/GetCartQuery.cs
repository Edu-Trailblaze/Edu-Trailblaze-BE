using Cart.Application.Common.Models;
using MediatR;
using Shared.SeedWork;

namespace Cart.Application.Features.V1.Carts.Queries.GetCarts
{
    public class GetCartQuery : IRequest<List<CartItemDTO>>
    {
        public string UserId { get; private set; } 
        public GetCartQuery(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }
    }
}
