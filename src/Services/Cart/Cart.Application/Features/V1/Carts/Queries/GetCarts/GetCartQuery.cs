using Cart.Domain.Entities;
using MediatR;
using Shared.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
