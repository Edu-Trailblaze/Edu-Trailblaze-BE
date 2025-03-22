using AutoMapper;
using Cart.Application.Common.Interfaces;
using Cart.Application.Common.Models;
using Cart.Application.Features.V1.Carts.Queries.GetCarts;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cart.Application.Features.V1.Carts.Queries.ViewCarts
{
    public class ViewCartQueryHandler : IRequestHandler<ViewCartQuery, CartInformation>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCartQueryHandler> _logger;
        public ViewCartQueryHandler(ICartRepository cartRepository, IMapper mapper, ILogger<GetCartQueryHandler> logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
        }
        private const string MethodName = "ViewCartQueryHandler";
        public async Task<CartInformation> Handle(ViewCartQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Begin {MethodName}");
            var orders = await _cartRepository.ViewCart(request.UserId);
            return orders;

        }
    }
}
