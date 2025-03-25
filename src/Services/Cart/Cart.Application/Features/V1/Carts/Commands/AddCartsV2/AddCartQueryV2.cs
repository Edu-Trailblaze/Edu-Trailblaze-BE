using Cart.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Application.Features.V1.Carts.Commands.AddCartsV2
{
    public class AddCartQueryV2 : IRequest<CartInformation>
    {
        public string UserId { get; private set; }
        public int CourseId { get; private set; }
        public AddCartQueryV2(string userId, int courseId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            CourseId = courseId;
        }
    }
}
