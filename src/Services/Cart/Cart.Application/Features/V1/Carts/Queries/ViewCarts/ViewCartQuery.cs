using Cart.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Application.Features.V1.Carts.Queries.ViewCarts
{
    public class ViewCartQuery : IRequest<CartInformation>
    {
        public string UserId { get; private set; }
        public ViewCartQuery(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }
    }
}
