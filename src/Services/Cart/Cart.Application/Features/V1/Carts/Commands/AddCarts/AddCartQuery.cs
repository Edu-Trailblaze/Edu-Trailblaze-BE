using MediatR;
using Entity = Cart.Domain.Entities;

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
