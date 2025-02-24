using Contracts.Domain;
using Entity =  Cart.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Application.Features.V1.Carts.Commands.AddCarts
{
    public class AddCartQuery : IRequest<Entity.Cart>
    {
        public Entity.Cart Cart { get; private set; }
        public AddCartQuery(Entity.Cart cart)
        {
            Cart = cart ?? throw new ArgumentNullException(nameof(cart));
        }
    }
}
