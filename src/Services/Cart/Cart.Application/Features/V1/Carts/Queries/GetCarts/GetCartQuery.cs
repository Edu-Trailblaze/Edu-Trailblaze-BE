using MediatR;
using Shared.SeedWork;

namespace Cart.Application.Features.V1.Carts.Queries.GetCarts
{
    public class GetCartQuery : IRequest<ApiResult<Cart.Domain.Entities.Cart>>
    {
        public string UserName { get; private set; }
        public GetCartQuery(string userName)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        }
    }
}
